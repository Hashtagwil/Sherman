using Assets.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Score;

public class TankController : MonoBehaviour, IModifiableEntity, ICanShootEntity
{
    private const float MOUSE_SLOWING_FACTOR = 1;

    private const float shieldActivationDuration = 1f;

    private Color shieldColor = new Color(1f, 1f, 0f, 0f);
    private Color innerShieldTarget = new Color(1f, 1f, 0f, 36f /255f);
    private Color outerShieldTarget = new Color(1f, 1f, 0f, 43f /255f);

    //Used values
    public float speed = 25.0f;
    public float turnSpeed = 45f;
    public int ModelLifeCount = 3;
    public float MaxProjectileDistance = 0f;
    public float ReloadTime = 1f;
    public float TurretSpeed = 5f;  // Should be between 2 (slow) and 5 (fast).  Faster than that lacks precision.
    public float TurretLength = 1.7f;
    public float sinkTime = 2.0f;
    private bool isDead = false;
    public bool hasFlag = false;

    [SerializeField] GameObject Turret;
    [SerializeField] GameObject Body;
    [SerializeField] GameObject Selection;
    [SerializeField] GameObject Projectile;
    [SerializeField] GameObject InnerShield;
    [SerializeField] GameObject OuterShield;
   
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip shootingAudio;
    [SerializeField] AudioClip shieldUpAudio;
    [SerializeField] AudioClip shieldDownAudio;
    [SerializeField] AudioClip hitSound;
    public AudioClip TurretAudio;

    public Slider SliderLife;
    [SerializeField] ParticleSystem smokeParticle;
    public ParticleSystem smokeParticleCritical;

    [SerializeField] GameObject explosion;

    private Animator animator;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private GameManager gameManager;
    public PlayerController PlayerController;
    public int Player = 1;
    public bool Fired = false;
    public float AIPathFindingGridSize = 0.5f;

    private List<IModifier> _modifiers = new List<IModifier>();
    private IFiringMode _firingMode;
    private ModifiableStats _baseStats;
    private ModifiableStats _modifiedStats;
    private GameObject _baseProjectile;

    public ModifiableStats BaseStats { get { return _baseStats; } }
    public ModifiableStats ModifiedStats { get { return _modifiedStats; } }
    public int IndexCarcass;

    public float RespawnTimer = 2f;
    private Vector3 SpawnPosition; 

    public NetPlayerAvatar netPlayerAvatar;

    public float[] RandomDirection;  

    Vector2 lastBullet;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        PlayerController = gameObject.GetComponent<PlayerController>();
        netPlayerAvatar = gameObject.GetComponentInParent<NetPlayerAvatar>();
        RandomDirection = new float[DataPersistence.Instance.data.CLUSTER_BOMB_PROJECTILE_COUNT];
    }

    void Start()
    {
        if (netPlayerAvatar != null)
        {
            netPlayerAvatar.GenerateNewRandomDirectionServerRpc();
        }

        _baseProjectile = Projectile;
        _baseStats = new ModifiableStats(ModelLifeCount);
        _firingMode = ScriptableObject.CreateInstance<DefaultFiringMode>();
        _modifiedStats = new ModifiableStats(_baseStats);
        if (SliderLife != null)
        {
            SliderLife.maxValue = ModifiedStats.Life;
            SliderLife.value = ModifiedStats.Life;
        }
        SpawnPosition = gameObject.transform.position;
    }

    public void RotateTurret(float angle)
    {
        if(Turret != null)
            Turret.transform.rotation = Quaternion.Lerp(Turret.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), TurretSpeed * MOUSE_SLOWING_FACTOR * Time.fixedDeltaTime);
    }

    public float RotateTurretFromController(float aimHorizontalInput, float lastAngle)
    {
        float angle = (lastAngle + -aimHorizontalInput * TurretSpeed) % 360; // Should be between 2f (slow) and ()
        Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), 360f);
        return angle;
    }

    public void InstantiateScore(ScorePosition scorePosition)
    {
        Score score = FindObjectsOfType<Score>().First(x => x.Position == scorePosition);
        score.Initialize(this, scorePosition);
    }

    public void Move(float horizontalInput, float forwardInput)
    {
        float speedMultiplier = 1f;
        float rotationMultiplier = 1f;
        if (ModifiedStats != null)
        {
            speedMultiplier = ModifiedStats.SpeedMultiplier;
            rotationMultiplier = ModifiedStats.SpeedMultiplier;
        }
        float forwardSpeed = speed * forwardInput * speedMultiplier;
        rb.AddRelativeForce(Vector2.right * forwardSpeed);

        //Rotation
        float turnDirection = turnSpeed * horizontalInput * rotationMultiplier;
        transform.Rotate(Vector3.back, Time.fixedDeltaTime * turnDirection);
    }

    public void Fire()
    {
        if (!Fired)
        { 
            StartCoroutine(FireRoutine());
        }
    }

    private void Update()
    {
        SetShield();
        SetSmokeParticle();
    }

    private void SetShield()
    {
        Color innerShieldColor = Color.black;
        Color outerShieldColor = Color.black;
        bool setColors = false;
        if (ModifiedStats.IsInvincible && ModifiedStats.LerpTime < 1)
        {
            audioSource.PlayOneShot(shieldUpAudio);
            innerShieldColor = Color.Lerp(shieldColor, innerShieldTarget, ModifiedStats.LerpTime);
            outerShieldColor = Color.Lerp(shieldColor, outerShieldTarget, ModifiedStats.LerpTime);
            setColors = true;
        }
        else if (!ModifiedStats.IsInvincible && ModifiedStats.LerpTime < 1)
        {
            audioSource.PlayOneShot(shieldDownAudio);
            innerShieldColor = Color.Lerp(innerShieldTarget, shieldColor, ModifiedStats.LerpTime);
            outerShieldColor = Color.Lerp(outerShieldTarget, shieldColor, ModifiedStats.LerpTime);
            setColors = true;
        }
        if (setColors)
        {
            InnerShield.GetComponent<SpriteRenderer>().color = innerShieldColor;
            OuterShield.GetComponent<SpriteRenderer>().color = outerShieldColor;
            ModifiedStats.LerpTime += Time.deltaTime / shieldActivationDuration;
        }
    }

    private void SetSmokeParticle()
    {
        if (smokeParticle != null)
        {
            if (ModifiedStats.Life == ModelLifeCount)
            {
                smokeParticle.Stop();
                smokeParticleCritical.Stop();
            } 
            else if (ModifiedStats.Life > 1 && ModifiedStats.Life == ModelLifeCount - 1)
            {
                smokeParticle.Play();
                smokeParticleCritical.Stop();
            } 
            else if (ModifiedStats.Life == 1)
            {
                smokeParticleCritical.Play();
            }
        }
    }

    public IEnumerator FireRoutine()
    {
        audioSource.PlayOneShot(shootingAudio);
        Fired = true;
        Vector3 pos = Turret.transform.position + Turret.transform.rotation * Vector3.right * transform.localScale.x * TurretLength;
        Quaternion rotation = Turret.transform.rotation;
        netPlayerAvatar.FireServerRpc(pos, rotation);

        // Fire locally to avoid delay from the server
        netPlayerAvatar.TakeShot(pos, rotation);

        yield return new WaitForSeconds(ReloadTime);
        Fired = false;
    }

    public void InstantiateBullet(Vector3 pos, Quaternion rotation)
    {
        lastBullet = pos;
        Projectile[] projectiles = _firingMode.FiringCanon(pos, Turret.transform.rotation, Projectile, ModifiedStats);
        foreach (Projectile projectile in projectiles)
        {
            projectile.Owner = this;
            projectile.MaxDistance = MaxProjectileDistance;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // The lerpTime check is used to be sure we are still invicible as long as the transition is not completed
        if (collision.gameObject.CompareTag("Projectile") && !ModifiedStats.IsInvincible && ModifiedStats.LerpTime >= 1f)
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            projectile.Explose();
            if (DataPersistence.Instance.TypeClient != TypeClient.Client)
            {
                netPlayerAvatar.TakeHitServerRpc(projectile.Owner.PlayerController.playerInfo.Player, true);
            }
        }
    }

    public void TakeHit(PlayerController playerSource)
    {
        if (!ModifiedStats.IsInvincible)
        {
            LoseLife(1.0f, playerSource);
            if (audioSource.isActiveAndEnabled) audioSource.PlayOneShot(hitSound);
        }
    }

    public void LoseLife(float lifeToLose, PlayerController playerSource = null)
    {
        ModifiedStats.Life -= lifeToLose;
        SliderLife.fillRect.gameObject.SetActive(true);
        SliderLife.value = ModifiedStats.Life;
        OnLifeLoss(playerSource);
    }

    private void OnLifeLoss(PlayerController playerSource)
    {
        if (ModifiedStats.Life <= 0)
        {
            SliderLife.fillRect.gameObject.SetActive(false);
            if (playerSource != null)
            {
                if (playerSource.playerInfo.teamColor != PlayerController.playerInfo.teamColor && DataPersistence.Instance.TypeClient != TypeClient.Client)
                {
                    FindObjectOfType<NetGame>().IncreaseKillCountClientRpc(playerSource.playerInfo.Player);
                } 
            }
            Die();
        }
        
    }

    public void GainLife(float lifeToGain)
    {
        ModifiedStats.Life += lifeToGain;
        SliderLife.fillRect.gameObject.SetActive(true);
        SliderLife.value = ModifiedStats.Life;
    }


    public void Die()
    {
        if(!isDead)
        {
            isDead = true;
            Explose();
            if(GameManager.SceneInstance.CurrentGameMode.isPermitRespawn() && DataPersistence.Instance.TypeClient != TypeClient.Client)
            {
                gameManager.RespawnPlayer(PlayerController.playerInfo);
            }
            if (PlayerController.DebugPlayerInfo != null)
            {
                Destroy(PlayerController.DebugPlayerInfo);
            }
            if (DataPersistence.Instance.data.LeaveCarcass && DataPersistence.Instance.TypeClient != TypeClient.Client)
            {
                GameObject netCarcass = Instantiate(Resources.Load("Net/NetCarcass") as GameObject, transform.position, transform.rotation);
                netCarcass.GetComponent<NetCarcass>().IndexCarcass = IndexCarcass;
                netCarcass.GetComponent<NetCarcass>().BodyColor = GetBodyColor();
                netCarcass.GetComponent<NetCarcass>().TurretColor = GetTurretColor();
                netCarcass.GetComponent<NetworkObject>().Spawn(true);
            }
            Destroy(gameObject);
        }
    }

    public void Explose()
    {
        GameObject newExplosion = Instantiate(explosion);
        newExplosion.transform.position = transform.position;
    }

    public void SelectBody()
    {
        Selection = Body;
    }

    public void SelectTurret()
    {
        Selection = Turret;
    }

    public void SetColorOfSelectedPart(Color color)
    {
        if (Selection == Turret)
        {
            SetTurretColor(color);
        }
        else if (Selection == Body)
        {
            SetBodyColor(color);
        }
    }

    private Color GetChildrenRendererFirstNonBlackColor(GameObject parent)
    {
        foreach (SpriteRenderer renderer in parent.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!IsColorBlackOrAlmost(renderer.color))
            {
                return renderer.color;
            }
        }
        return Color.white;
    }

    public Color GetBodyColor()
    {
        return GetChildrenRendererFirstNonBlackColor(Body);
    }

    public Color GetTurretColor()
    {
        return GetChildrenRendererFirstNonBlackColor(Turret);
    }

    public void SetChildrenNonBlackRenderersToColor(GameObject parent, Color color)
    {
        foreach (SpriteRenderer renderer in parent.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!IsColorBlackOrAlmost(renderer.color))
            {
                renderer.color = color;
            }
        }
    }

    public bool IsColorBlackOrAlmost(Color color)
    {
        bool almostBlack = true;
        if (color.r > 0.06f || color.g > 0.06f || color.b > 0.06f)
        {
            almostBlack = false;
        }
        return almostBlack;
    }

    public void SetTurretColor(Color color)
    {
        // turret need to be lighter than the body
        color = new Color(color.r * 1.2f, color.g * 1.2f, color.b * 1.2f);
        SetChildrenNonBlackRenderersToColor(Turret, color);
    }

    public void SetBodyColor(Color color)
    {
        color = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
        SetChildrenNonBlackRenderersToColor(Body, color);
    }

    public void AddModifer(IModifier modifier)
    {
        _modifiers.Add(modifier);
        this.ApplyModifiers();

        StartCoroutine(DeactivatePowerUpOnDelay(modifier));
    }

    public void RemoveModifier(IModifier modifier)
    {
        if (_modifiers.Remove(modifier))
        {
            modifier.Deactivate(ModifiedStats);
        }
    }

    private void ApplyModifiers()
    {
        foreach (var mod in _modifiers)
        {
            mod.Activate(ModifiedStats);
        }
    }

    public void ChangeFiringMode(IFiringMode firingMode)
    {
        _firingMode = firingMode;
        if (firingMode.GetType() == typeof(TripleShot))
            setTripleShotStatus(true);

        if(firingMode.newProjectile != null)
        {
            Projectile = firingMode.newProjectile;
        }

        if (firingMode.GetType() != typeof(DefaultFiringMode))
            StartCoroutine(ResetFiringMode(firingMode));
    }

    IEnumerator ResetFiringMode(IFiringMode firingMode)
    {
        yield return new WaitForSeconds(firingMode.Duration);

        if (firingMode.GetType() == typeof(TripleShot))
            setTripleShotStatus(false);

        Projectile = _baseProjectile;
        ChangeFiringMode(ScriptableObject.CreateInstance<DefaultFiringMode>());
    }

    private void setTripleShotStatus(bool enabled)
    {
        var renderers = Turret.GetComponentsInChildren<SpriteRenderer>()
            .ToList()
            .FindAll(x => x.tag == "ActiveOnDemand");
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = enabled;
        }
    }

    IEnumerator DeactivatePowerUpOnDelay(IModifier powerUp)
    {
        yield return new WaitForSeconds(powerUp.Duration);
        RemoveModifier(powerUp);
    }

    public void StartSpawnInvincibility(float delay)
    {
        _modifiedStats.IsInvincible = true;
        _modifiedStats.LerpTime = 0;
        StartCoroutine(StopSpawnInvincibility(delay));
    }

    IEnumerator StopSpawnInvincibility(float delay)
    {
        yield return new WaitForSeconds(delay);
        _modifiedStats.IsInvincible = false;
        _modifiedStats.LerpTime = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            Destroy(collision.gameObject);
        }
    }

    public IEnumerator StartSinking()
    {
        yield return new WaitForSeconds(sinkTime);
        if (ModifiedStats.IsInWater)
        {
            if (GameManager.SceneInstance.CurrentGameMode.isPermitRespawn() && DataPersistence.Instance.TypeClient != TypeClient.Client)
            {
                FindObjectOfType<NetGame>().SinkClientRpc(transform.position, transform.rotation);
                gameManager.RespawnPlayer(PlayerController.playerInfo);
                if (PlayerController.DebugPlayerInfo != null)
                {
                    Destroy(PlayerController.DebugPlayerInfo);
                }
                Destroy(gameObject);
            }
        }
        
    }

    public void TurnTurretTowardsPosition(Vector2 position)
    {
        // Adjust Turret rotation
        GameObject turret = GetComponent<TankController>().Turret;
        Vector3 turretPosNoZ = new Vector3(turret.transform.position.x, turret.transform.position.y, 0f);
        Vector3 targetPosNoZ = new Vector3(position.x, position.y, 0f);
        Vector3 direction = (targetPosNoZ - turretPosNoZ);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        netPlayerAvatar.UpdateTurretServerRpc(angle);
    }

    private void OnDestroy()
    {
        if (netPlayerAvatar != null)
            Destroy(netPlayerAvatar.gameObject);
    }
    
    internal void FlagCaptured()
    {
        hasFlag = false;
    }

    internal void FlagTaken()
    {
        hasFlag = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * TurretLength);
    }

    public void UpdateDrag()
    {
        rb.drag = ModifiedStats.Drag;
        rb.angularDrag = ModifiedStats.AngularDrag;
    }

    public void GetNewRandomDirection()
    {
        netPlayerAvatar.GenerateNewRandomDirectionServerRpc();
    }
}
