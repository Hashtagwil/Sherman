using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterBombProjectile : Projectile
{
    [SerializeField] GameObject projectile; 
    private const float TIMER = 0.75f;
    private Collider2D collider;

    private bool hasExploded = false;

    public void Start()
    {
        StartCoroutine(TicTacTic());
        collider = GetComponent<CircleCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private IEnumerator TicTacTic()
    {
        yield return new WaitForSeconds(TIMER);
        ClusterExplosion();
    }

    private void ClusterExplosion()
    {
        if(!hasExploded)
        {
            hasExploded = true;
            collider.enabled = false;
            for(int i = 0; i < DataPersistence.Instance.data.CLUSTER_BOMB_PROJECTILE_COUNT; i++)
            {
                var rotation = transform.rotation;
                var euler = transform.eulerAngles;
                euler.z = Owner.RandomDirection[i];
                rotation.eulerAngles = euler;
                GameObject projectileInstance = Instantiate(projectile, transform.position, rotation);
                var projectileComponent = projectileInstance.GetComponent<Projectile>();
                projectileComponent.canBounce = false;
                projectileComponent.Owner = Owner;
                projectileComponent.MaxDistance = Owner.MaxProjectileDistance;
            }
            Owner.GetNewRandomDirection();
            Explose();
        }
    }

    public override void Explose()
    {
        ClusterExplosion();
        base.Explose();
    }


}
