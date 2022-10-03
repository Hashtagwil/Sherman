using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterBomb : MonoBehaviour, IFiringMode
{
    private float _duration = 15.0f;
    private TankController tankController;
    [SerializeField] GameObject ClusterBombProjectile; 

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
            return ClusterBombProjectile;
        }
    }

    public Projectile[] FiringCanon(Vector3 position, Quaternion rotation, GameObject projectilePrefab, ModifiableStats stats)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.GetComponent<Projectile>().canBounce = false;
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
