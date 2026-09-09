using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeAnimation : MonoBehaviour
{
    private CanvasGroup cg;

    private void Awake() => cg = GetComponent<CanvasGroup>();

    public void FadeIn(float duration = 0.25f) => Fade(1f, duration);
    public void FadeOut(float duration = 0.25f) => Fade(0f, duration);

    public void Fade(float targetAlpha, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float target, float duration)
    {
        float start = cg.alpha;
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        cg.alpha = target;
    }
}