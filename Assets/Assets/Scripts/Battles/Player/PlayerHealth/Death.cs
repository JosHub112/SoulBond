using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject deathScreen; // Drag your DeathScreen GameObject here

    private bool isDead = false;

    private void Start()
    {
        // Ensure the death screen starts hidden
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        // Call the death sequence directly as a normal function
        HandleDeath();
    }

    private void HandleDeath()
    {
        Debug.Log("Player Died!");

        // 1. Hide the player sprite (optional)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // 2. Turn on the "YOU DIED" UI screen and leave it open
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}