using System.Collections;
using UnityEngine;

public class ObjectAnimations : MonoBehaviour
{
    private Vector3 baseScale;
    private Vector3 lastShakeOffset;
    private Coroutine shakeCoroutine;
    private Coroutine bounceCoroutine;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void TriggerShake(float duration = 0.2f, float strength = 0.2f)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    public void TriggerBounce(float duration = 0.2f, float stretchFactor = 1.25f)
    {
        if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
        bounceCoroutine = StartCoroutine(BounceRoutine(duration, stretchFactor));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // 1. Revert previous frame's offset so your movement script doesn't drift
            transform.position -= lastShakeOffset;

            // 2. Generate new random offset
            lastShakeOffset = (Vector3)(Random.insideUnitCircle * strength);

            // 3. Apply new offset
            transform.position += lastShakeOffset;

            yield return null;
        }

        // Clean up when finished
        transform.position -= lastShakeOffset;
        lastShakeOffset = Vector3.zero;
    }

    private IEnumerator BounceRoutine(float duration, float stretch)
    {
        Vector3 targetScale = new Vector3(baseScale.x * stretch, baseScale.y / stretch, baseScale.z);
        float half = duration / 2f;

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(baseScale, targetScale, t / half);
            yield return null;
        }
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(targetScale, baseScale, t / half);
            yield return null;
        }
        transform.localScale = baseScale;
    }
}