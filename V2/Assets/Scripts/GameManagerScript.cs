using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI;
    public AudioClip deathSound; // Dźwięk umierania
    private AudioSource audioSource; // Komponent AudioSource
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Upewnij się, że gameOverUI jest nieaktywny na starcie
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Game Over UI is not assigned in the GameManagerScript!");
        }
        // Inicjalizacja AudioSource
        audioSource = GetComponent<AudioSource>();
        // if (audioSource == null)
        // {
        //     Debug.LogError("AudioSource component missing from the GameManager!");
        // }
    }


    public void gameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game Over UI is not assigned in the GameManagerScript!");
        }

        // Odtwórz dźwięk umierania
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        else
        {
            Debug.LogWarning("Death sound or AudioSource not set in the GameManagerScript!");
        }
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quit()
    {
        Application.Quit();
    }
}
