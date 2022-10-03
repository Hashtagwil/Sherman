using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBall : MonoBehaviour, IFiringMode
{
    private TankController tankController;
    private float scale = 1.2f;
    private float offset = 0.25f;

    private float _duration = 20.0f;
    public float Duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = value;
        }
    }

    public GameObject newProjectile
    {
        get
        {
            return null;
        }
    }

    public Projectile[] FiringCanon(Vector3 position, Quaternion rotation, GameObject projectilePrefab, ModifiableStats stats)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.transform.position += projectile.transform.right * offset;
        projectile.transform.localScale = new Vector3(scale, scale, position.z);
        projectile.GetComponent<Projectile>().canBounce = stats.BulletCanBounce;
        projectile.GetComponent<Projectile>().NbBounceRemaining = stats.NbBounces;

        return new Projectile[] { projectile.GetComponent<Projectile>() };
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("AI_Tank"))
        {
            tankController = collision.gameObject.GetComponent<TankController>();
            tankController.ChangeFiringMode(this);
        }
    }
}
