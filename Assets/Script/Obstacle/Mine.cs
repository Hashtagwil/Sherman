using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] SpriteRenderer spriteRenderer;

    private Color darkishRed = new Color(178,0,0);

    void Start()
    {
        // Start the flashing of each mine randomly, so they flash asynchronously
        Invoke("AlternColor", Random.Range(1, 6));
    }

    void AlternColor()
    {
        if (spriteRenderer.color == Color.black)
            spriteRenderer.color = darkishRed;
        else
            spriteRenderer.color = Color.black;

        Invoke("AlternColor", Random.Range(0.5f, 1.0f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            if (tank.netPlayerAvatar.IsOwner)
            {
                tank.netPlayerAvatar.TakeHitServerRpc(tank.PlayerController.playerInfo.Player, false);
                tank.TakeHit(tank.PlayerController);

                GameObject.FindObjectOfType<NetGame>().DestroyObjectServerRpc(gameObject.name);
                // Locally, destroy on the spot for a better effect
                Explose();
            }
        }

        if (collision.gameObject.tag == "Projectile")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
        }
    }

    public void Explose()
    {
        GameObject newExplosion = Instantiate(explosion);
        newExplosion.transform.position = transform.position;
        Destroy(gameObject);
    }
}
