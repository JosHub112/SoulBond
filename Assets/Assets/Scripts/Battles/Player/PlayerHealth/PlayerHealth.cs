using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    [Header("UI Reference")]
    [SerializeField] private Sprite fullHeartSprite;  // Drag Full Heart Sprite here
    [SerializeField] private Sprite emptyHeartSprite; // Drag Empty Heart Sprite here
    [SerializeField] private Transform heartsContainer; // Drag the 'Hearts' parent object here

    [Header("I-Frames")]
    [SerializeField] private float iFrameDuration = 1f;
    private bool isInvincible = false;

    [SerializeField] private ObjectAnimations Shake;


    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartUI();
    }

    public void TakeDamage(int damage = 1)
    {
        if (isInvincible || currentHealth <= 0) return;

        Debug.Log($"Hit! Shake reference is null? {Shake == null}");

        if (Shake != null)
        {
            Shake.TriggerShake(0.3f, 0.3f); 
            Shake.TriggerBounce(0.3f, 1.2f);
        }

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

        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            Transform heartChild = heartsContainer.GetChild(i);

            // Keep all heart objects visible
            heartChild.gameObject.SetActive(true);

            // Swap sprite based on current health (UI Image)
            if (heartChild.TryGetComponent<Image>(out var heartImage))
            {
                heartImage.sprite = (i < currentHealth) ? fullHeartSprite : emptyHeartSprite;
            }
            // Fallback for World-Space SpriteRenderer (if not using UI Canvas)
            else if (heartChild.TryGetComponent<SpriteRenderer>(out var heartSprite))
            {
                heartSprite.sprite = (i < currentHealth) ? fullHeartSprite : emptyHeartSprite;
            }
        }
    }
}