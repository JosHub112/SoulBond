using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    [Header("UI Reference")]
    [SerializeField] private Transform heartsContainer; // Drag the 'Hearts' parent object here

    [Header("I-Frames")]
    [SerializeField] private float iFrameDuration = 1f;
    private bool isInvincible = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartUI();
    }

    public void TakeDamage(int damage = 1)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHeartUI();

        if (currentHealth <= 0)
        {
            // Trigger separate death script
            PlayerDeath deathScript = GetComponent<PlayerDeath>();
            if (deathScript != null)
            {
                deathScript.TriggerDeath();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }
        else
        {
            StartCoroutine(IFrameRoutine());
        }
    }

    private IEnumerator IFrameRoutine()
    {
        isInvincible = true;

        // Optional visual flash
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(iFrameDuration);
            sr.color = Color.white;
        }
        else
        {
            yield return new WaitForSeconds(iFrameDuration);
        }

        isInvincible = false;
    }

    public void UpdateHeartUI()
    {
        if (heartsContainer == null) return;

        // Toggles heart objects on/off based on current HP count
        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            heartsContainer.GetChild(i).gameObject.SetActive(i < currentHealth);
        }
    }
}