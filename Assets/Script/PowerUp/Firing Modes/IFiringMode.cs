using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFiringMode
{
    float Duration { get; set; }
    Projectile[] FiringCanon(Vector3 position, Quaternion rotation, GameObject projectilePrefab, ModifiableStats stats);
    GameObject newProjectile { get; }
}
