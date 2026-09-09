using System.Collections;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 basePos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;
    }

    public void TriggerShake(float duration = 0.2f, float strength = 10f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            rect.anchoredPosition = basePos + (Random.insideUnitCircle * strength);
            yield return null;
        }
        rect.anchoredPosition = basePos;
    }
}