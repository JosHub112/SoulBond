using System.Collections;
using UnityEngine;

public class UIPopupAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    private Vector3 baseScale;

    private void Awake() => baseScale = transform.localScale;

    public void OpenPopup()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Animate(Vector3.zero, baseScale));
    }

    public void ClosePopup()
    {
        StopAllCoroutines();
        StartCoroutine(Animate(transform.localScale, Vector3.zero, () => gameObject.SetActive(false)));
    }

    private IEnumerator Animate(Vector3 start, Vector3 end, System.Action onComplete = null)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            float progress = Mathf.SmoothStep(0, 1, t / duration);
            transform.localScale = Vector3.Lerp(start, end, progress);
            yield return null;
        }
        transform.localScale = end;
        onComplete?.Invoke();
    }
}