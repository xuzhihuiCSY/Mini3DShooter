using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        GetComponent<FPSController>().enabled = false;

        GameOverUI gameOver = FindFirstObjectByType<GameOverUI>();
        if (gameOver != null)
        {
            gameOver.ShowGameOver();
        }
    }

}
