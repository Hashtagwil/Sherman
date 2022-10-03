using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool canBounce = false;
    public TankController Owner;
    public int NbBounceRemaining = 0;
    public Sprite IntactSprite;
    public Sprite OneCollisionRemainingSprite;
    public Sprite TwoCollisionRemainingSprite;
    public Collider2D LastCollisionRigidBody;
    public float MaxDistance;
    public Vector2 ShootingPosition;

    float speed = 300f;
    float magnitude = 6f;
    protected Rigidbody2D rb;

    [SerializeField] GameObject explosion;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddRelativeForce(Vector3.right * speed);
        ShootingPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") && collision.gameObject.GetComponent<Projectile>().Owner != this.Owner)
        {
            Explose();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (canBounce)
            {
                NbBounceRemaining--;
                if (NbBounceRemaining == 1)
                {
                    GetComponent<SpriteRenderer>().sprite = OneCollisionRemainingSprite;
                    GetComponent<CircleCollider2D>().enabled = false;
                    LastCollisionRigidBody.enabled = true;
                }
                else if (NbBounceRemaining == 2)
                {
                    GetComponent<SpriteRenderer>().sprite = TwoCollisionRemainingSprite;
                }
            }
            if (NbBounceRemaining == 0 || !canBounce)
            {
                Explose();
            }
        }
    }

    public virtual void Explose()
    { 
        GameObject newExplosion = Instantiate(explosion);
        newExplosion.transform.position = transform.position;
        Destroy(gameObject);
    }

    public void Update()
    {
		if (MaxDistance == 0 || UnityUtils.IsNullOrDestroyed(Owner)) return;

        float distanceWithOwner = Vector3.Distance(transform.position, ShootingPosition);
        if (distanceWithOwner > MaxDistance)
        {
            Explose();
        }
    }

    public void FixedUpdate()
    {
        //We're making sure the speed of the bullet is constant; 
        if (rb.velocity.sqrMagnitude != magnitude * magnitude)
            rb.velocity = rb.velocity.normalized * magnitude;
    }
}
