using System.Collections;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public int damageToPlayer = 10; // Damage to the player
    public HealthSystem playerHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage the player
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damageToPlayer);
        }
    }
}