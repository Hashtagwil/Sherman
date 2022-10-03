using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultFiringMode : ScriptableObject, IFiringMode
{
    private float _duration = 0;
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
        projectile.GetComponent<Projectile>().canBounce = stats.BulletCanBounce;
        projectile.GetComponent<Projectile>().NbBounceRemaining = stats.NbBounces;

        return new Projectile[] { projectile.GetComponent<Projectile>() };
    }
}
