using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private float restartDelay = 1f; // Delay before restarting scene

    private bool isDead = false;

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Debug.Log("Player Died!");

        // Optional: Disable player sprite while waiting
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Wait for the delay duration
        yield return new WaitForSeconds(restartDelay);

        // Restart active scene
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}