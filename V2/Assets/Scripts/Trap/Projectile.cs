using System.Collections;
using UnityEngine;

public class Projectile : Trap
{
    public Vector2 direction;
    public int damageToPlayer = 10; // Damage to the player
    public int damageToObjects = 10; // Damage to other objects (e.g., enemies, destructible walls)
    public float movingSpeed;
    public float destroyTime;
    public HealthSystem playerHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.collider.gameObject.layer);

        // Damage the player
        if (layerName == "Player")
        {
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }

        // Damage other objects with a HealthSystem component
        HealthSystem healthSystem = collision.collider.GetComponent<HealthSystem>();
        if(collision.collider.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damageToPlayer);
        }

        if (healthSystem != null)
        {
            playerHealth.TakeDamage(damageToPlayer);
        }

        // Destroy the projectile after collision
        Destroy(gameObject);
    }

    public override void trigger()
    {
        Vector2 newVelocity = direction.normalized * movingSpeed;
        gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;

    }

}

