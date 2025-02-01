using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health;
    public int maxHealth = 10; // Maximum health of the object
    private int currentHealth; // Current health of the object

    private bool isDead;

    public GameManagerScript gameManager;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    // Method to apply damage to the object
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce health by the damage amount

        // Check if the object is dead
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            gameManager.gameOver();
            Die(); // Call the Die method if health drops to 0 or below
        }

        Debug.Log(gameObject.name + " took " + damage + " damage! Health remaining: " + currentHealth);
    }

    // Method to handle the object's death
    private void Die()
    {
        Debug.Log(gameObject.name + " died!");

        // Optional: Play a death animation or effect
        // animator.SetTrigger("Die");

        // Destroy the object
        Destroy(gameObject);
    }
}