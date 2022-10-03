using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : MonoBehaviour, IFiringMode
{
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

    private TankController tankController;
    private float offset = 0.25f;
    private float angleOffset = 8;

    public Projectile[] FiringCanon(Vector3 position, Quaternion rotation, GameObject projectilePrefab, ModifiableStats stats)
    {
        bool canBounce = stats.BulletCanBounce;
        GameObject projectile1 = Instantiate(projectilePrefab, position, rotation); // Projectile 1 is the default centered projectile
        projectile1.GetComponent<Projectile>().canBounce = canBounce;

        GameObject projectile2
            = Instantiate(projectilePrefab
            , position + projectile1.transform.up * offset
            , rotation *= Quaternion.Euler(0, 0, angleOffset));
        projectile2.GetComponent<Projectile>().canBounce = canBounce;

        GameObject projectile3
            = Instantiate(projectilePrefab
            , position - projectile1.transform.up * offset
            , rotation *= Quaternion.Euler(0, 0, -(2 * angleOffset)));
        projectile3.GetComponent<Projectile>().canBounce = canBounce;

        if (canBounce)
        {
            projectile1.GetComponent<Projectile>().NbBounceRemaining = stats.NbBounces;
            projectile2.GetComponent<Projectile>().NbBounceRemaining = stats.NbBounces;
            projectile3.GetComponent<Projectile>().NbBounceRemaining = stats.NbBounces;
        }

        return new Projectile[] { projectile1.GetComponent<Projectile>(), projectile2.GetComponent<Projectile>(), projectile3.GetComponent<Projectile>() };
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
