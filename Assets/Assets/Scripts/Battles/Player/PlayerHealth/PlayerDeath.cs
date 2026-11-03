using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Put this on the player (next to PlayerHealth). PlayerHealth calls TriggerDeath() at 0 HP.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private DeathScreen deathScreen;

    [Tooltip("Scripts to switch off when the player dies (movement, attacks, ...).")]
    [SerializeField] private Behaviour[] disableOnDeath;

    private bool isDead;

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        foreach (Behaviour b in disableOnDeath)
        {
            if (b != null) b.enabled = false;
        }

        if (deathScreen != null)
        {
            deathScreen.Show();
        }
        else
        {
            Debug.LogWarning("PlayerDeath: no DeathScreen assigned, reloading scene instead.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
