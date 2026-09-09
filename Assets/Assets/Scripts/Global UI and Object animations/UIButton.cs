using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.9f;
    [SerializeField] private float speed = 15f;

    private Vector3 baseScale;
    private Vector3 targetScale;

    private void Awake() => baseScale = targetScale = transform.localScale;

    private void Update() => transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);

    public void OnPointerEnter(PointerEventData _) => targetScale = baseScale * hoverScale;
    public void OnPointerExit(PointerEventData _) => targetScale = baseScale;
    public void OnPointerDown(PointerEventData _) => targetScale = baseScale * clickScale;
    public void OnPointerUp(PointerEventData _) => targetScale = baseScale * hoverScale;
}