using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject presentedByScreen;
    public GameObject homeScreen;

    [Header("UI Canvas Groups")]
    public CanvasGroup presentedByCanvasGroup;
    public CanvasGroup homeScreenCanvasGroup;

    [Header("Settings")]
    public float displayTime = 3f;
    public float fadeDuration = 1f;

    void Start()
    {
        if (presentedByCanvasGroup != null)
        {
            presentedByCanvasGroup.alpha = 1f;
            presentedByCanvasGroup.gameObject.SetActive(true);
        }

        if (homeScreenCanvasGroup != null)
        {
            homeScreenCanvasGroup.alpha = 0f;
            homeScreenCanvasGroup.gameObject.SetActive(false);
        }

        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(displayTime);

        float timer = 0f;

        if (homeScreenCanvasGroup != null)
        {
            homeScreenCanvasGroup.gameObject.SetActive(true);
            homeScreenCanvasGroup.alpha = 0f;
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // Fade out Presented By, Fade in HomeScreen
            if (presentedByCanvasGroup != null)
                presentedByCanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            if (homeScreenCanvasGroup != null)
                homeScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }

        // Clean up final states
        if (presentedByCanvasGroup != null)
            presentedByCanvasGroup.gameObject.SetActive(false);
    }

    IEnumerator ShowHomeScreenRoutine()
    {
        yield return new WaitForSeconds(displayTime);

        
        if (presentedByScreen != null) presentedByScreen.SetActive(false);
        if (homeScreen != null) homeScreen.SetActive(true);
    }


    public void StartGame()
    {
       
        SceneManager.LoadScene(1);
    }
}