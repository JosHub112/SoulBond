using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Undertale-style effects for the battle UI.
///
/// Everything here is driven by one fixed-rate "tick" (effectFps) instead of every frame,
/// so movement, scaling and blinking all happen in choppy steps. That is what gives the
/// jittery, low-framerate feel.
///
/// Covers:
///  - Panels and buttons blinking / wobbling in, one after another
///  - Constant tiny jitter on idle buttons, stronger jitter + yellow tint on hovered/selected ones
///  - A click shake
///  - The black transition panel (encounter flash, choppy fades)
///
/// Put this on an object that stays active (for example the Canvas root).
/// </summary>
public class BattleUIEffects : MonoBehaviour
{
    [Header("Choppy Animation")]
    [Tooltip("How many times per second the UI updates. Lower = choppier.")]
    [SerializeField, Min(1)] private int effectFps = 15;

    [Header("Jitter Strength (UI units)")]
    [SerializeField] private float idleJitter = 1f;
    [SerializeField] private float selectedJitter = 3f;
    [SerializeField] private float introJitter = 6f;
    [SerializeField] private float clickJitter = 8f;

    [Header("Pop In")]
    [SerializeField] private float introDuration = 0.3f;
    [Tooltip("Delay between each button popping in.")]
    [SerializeField] private float staggerDelay = 0.06f;
    [Tooltip("How far buttons over/undershoot their size while popping in.")]
    [SerializeField] private float introScaleKick = 0.35f;

    [Header("Click")]
    [SerializeField] private float clickDuration = 0.25f;
    [SerializeField] private float clickScaleKick = 0.15f;

    [Header("Selection")]
    [Tooltip("Multiplied with the button's image colour while hovered/selected. Set to white to disable.")]
    [SerializeField] private Color selectedTint = new Color(1f, 0.92f, 0.1f);

    [Header("Transition")]
    [SerializeField] private CanvasGroup transitionPanel; // black panel with a CanvasGroup
    [SerializeField] private float fadeDuration = 0.5f;
    [Tooltip("Number of visible steps in a fade. Lower = choppier.")]
    [SerializeField, Min(1)] private int fadeSteps = 6;
    [Tooltip("How many times the screen blinks black when a battle starts.")]
    [SerializeField, Min(1)] private int flashBlinks = 3;
    [SerializeField] private float flashInterval = 0.07f;

    // ---------------------------------------------------------------------
    // Internal state
    // ---------------------------------------------------------------------

    private class FxElement
    {
        public RectTransform Rect;
        public CanvasGroup Group;
        public Graphic Graphic;
        public bool IsPanel;

        // Pose to return to when the effect stops
        public Vector2 RestPosition;
        public Vector3 RestScale;
        public float RestAlpha;
        public Color RestColor;

        public bool Live;
        public bool Hovered;
        public float Age;
        public float Delay;
        public float ClickAge = -1f;
        public int Tick;
    }

    private readonly Dictionary<RectTransform, FxElement> elements = new Dictionary<RectTransform, FxElement>();
    private readonly List<FxElement> liveElements = new List<FxElement>();
    private float tickTimer;

    private float TickInterval => 1f / Mathf.Max(1, effectFps);

    private void Awake()
    {
        // Start fully transparent and let clicks pass through
        if (transitionPanel != null)
        {
            transitionPanel.alpha = 0f;
            transitionPanel.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        tickTimer += Time.unscaledDeltaTime;
        if (tickTimer < TickInterval) return;

        float dt = tickTimer;
        tickTimer = 0f;

        for (int i = liveElements.Count - 1; i >= 0; i--)
        {
            FxElement el = liveElements[i];

            if (el.Rect == null)
            {
                liveElements.RemoveAt(i);
                continue;
            }

            Animate(el, dt);
        }
    }

    // ---------------------------------------------------------------------
    // Panel / button effects
    // ---------------------------------------------------------------------

    /// <summary>
    /// Call right after the panel and buttons have been activated.
    /// The panel blinks in, then the buttons pop in one after another.
    /// </summary>
    public void PlayShow(RectTransform panel, IReadOnlyList<Button> buttons)
    {
        if (panel != null)
        {
            // Let any layout group place its children before we record their rest positions
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            Begin(GetOrCreatePanel(panel), 0f);
        }

        int shown = 0;
        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];
            if (button == null || !button.gameObject.activeInHierarchy) continue;

            Begin(GetOrCreateButton(button), shown * staggerDelay);
            shown++;
        }
    }

    /// <summary>
    /// Call just before the panel and buttons are deactivated.
    /// Instantly stops the effects and puts everything back in its resting pose.
    /// </summary>
    public void Stop(RectTransform panel, IReadOnlyList<Button> buttons)
    {
        if (panel != null && elements.TryGetValue(panel, out FxElement panelElement))
        {
            Restore(panelElement);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null) continue;

            RectTransform rect = buttons[i].transform as RectTransform;
            if (rect != null && elements.TryGetValue(rect, out FxElement el))
            {
                Restore(el);
            }
        }
    }

    private FxElement GetOrCreatePanel(RectTransform rect)
    {
        if (elements.TryGetValue(rect, out FxElement existing)) return existing;
        return CreateElement(rect, true);
    }

    private FxElement GetOrCreateButton(Button button)
    {
        RectTransform rect = (RectTransform)button.transform;
        if (elements.TryGetValue(rect, out FxElement existing)) return existing;

        FxElement el = CreateElement(rect, false);
        el.Graphic = button.targetGraphic;

        // Hover / keyboard-controller selection
        if (!button.TryGetComponent(out EventTrigger trigger))
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        AddTrigger(trigger, EventTriggerType.PointerEnter, _ => el.Hovered = true);
        AddTrigger(trigger, EventTriggerType.PointerExit, _ => el.Hovered = false);
        AddTrigger(trigger, EventTriggerType.Select, _ => el.Hovered = true);
        AddTrigger(trigger, EventTriggerType.Deselect, _ => el.Hovered = false);

        // Click shake
        button.onClick.AddListener(() =>
        {
            if (el.Live) el.ClickAge = 0f;
        });

        return el;
    }

    private FxElement CreateElement(RectTransform rect, bool isPanel)
    {
        if (!rect.TryGetComponent(out CanvasGroup group))
        {
            group = rect.gameObject.AddComponent<CanvasGroup>();
        }

        FxElement el = new FxElement
        {
            Rect = rect,
            Group = group,
            IsPanel = isPanel,
        };

        elements.Add(rect, el);
        return el;
    }

    private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void Begin(FxElement el, float delay)
    {
        // If it's already animating, put it back first so we don't record a jittered pose as "rest"
        if (el.Live) Restore(el);

        el.RestPosition = el.Rect.anchoredPosition;
        el.RestScale = el.Rect.localScale;
        el.RestAlpha = el.Group.alpha;
        if (el.Graphic != null) el.RestColor = el.Graphic.color;

        el.Age = 0f;
        el.Delay = delay;
        el.ClickAge = -1f;
        el.Tick = 0;
        el.Live = true;
        liveElements.Add(el);

        // Apply the first frame right away so nothing shows at full size before the first tick
        Animate(el, 0f);
    }

    private void Restore(FxElement el)
    {
        if (!el.Live) return;

        el.Live = false;
        el.Hovered = false;
        el.ClickAge = -1f;
        liveElements.Remove(el);

        if (el.Rect == null) return;

        el.Rect.anchoredPosition = el.RestPosition;
        el.Rect.localScale = el.RestScale;
        el.Group.alpha = el.RestAlpha;
        if (el.Graphic != null) el.Graphic.color = el.RestColor;
    }

    private void Animate(FxElement el, float dt)
    {
        el.Age += dt;

        // Flips every tick. Drives the blinking and the over/undershoot wobble.
        bool flip = (el.Tick++ & 1) == 0;
        float sign = flip ? 1f : -1f;

        if (el.ClickAge >= 0f)
        {
            el.ClickAge += dt;
            if (el.ClickAge >= clickDuration) el.ClickAge = -1f;
        }

        float introTime = el.Age - el.Delay;

        // Waiting for its turn in the stagger: invisible and unclickable
        if (introTime < 0f)
        {
            el.Group.alpha = 0f;
            el.Rect.localScale = Vector3.zero;
            return;
        }

        float strength;
        float scale = 1f;
        float alpha = 1f;

        if (introTime < introDuration)
        {
            // Pop in: blink, shake and wobble in size, settling over introDuration
            float remaining = 1f - introTime / introDuration;

            strength = introJitter * remaining;
            alpha = flip ? 0.35f : 1f;
            if (!el.IsPanel) scale = 1f + sign * introScaleKick * remaining;
        }
        else if (el.IsPanel)
        {
            strength = 0f;
        }
        else
        {
            strength = el.Hovered ? selectedJitter : idleJitter;

            if (el.ClickAge >= 0f)
            {
                float remaining = 1f - el.ClickAge / clickDuration;

                strength = Mathf.Max(strength, clickJitter * remaining);
                scale = 1f + sign * clickScaleKick * remaining;
            }
        }

        el.Rect.anchoredPosition = el.RestPosition + Jitter(strength);
        el.Rect.localScale = el.RestScale * scale;
        el.Group.alpha = el.RestAlpha * alpha;

        if (el.Graphic != null)
        {
            el.Graphic.color = el.Hovered ? el.RestColor * selectedTint : el.RestColor;
        }
    }

    /// <summary>Random offset snapped to whole units, so it moves in pixel-like steps.</summary>
    private static Vector2 Jitter(float strength)
    {
        if (strength <= 0f) return Vector2.zero;

        return new Vector2(
            Mathf.Round(Random.Range(-strength, strength)),
            Mathf.Round(Random.Range(-strength, strength)));
    }

    // ---------------------------------------------------------------------
    // Screen transition
    // ---------------------------------------------------------------------

    /// <summary>
    /// Covers the screen in black.
    /// flash = true blinks the screen like Undertale's encounter flash, then holds black.
    /// flash = false does a choppy stepped fade.
    /// </summary>
    public IEnumerator FadeToBlack(bool flash)
    {
        if (transitionPanel == null) yield break;

        // Block clicks on UI underneath while the screen is covered
        transitionPanel.blocksRaycasts = true;

        if (flash)
        {
            for (int i = 0; i < flashBlinks; i++)
            {
                transitionPanel.alpha = 1f;
                yield return new WaitForSecondsRealtime(flashInterval);

                transitionPanel.alpha = 0f;
                yield return new WaitForSecondsRealtime(flashInterval);
            }

            transitionPanel.alpha = 1f;
        }
        else
        {
            yield return SteppedFade(0f, 1f);
        }
    }

    /// <summary>Uncovers the screen with a choppy stepped fade.</summary>
    public IEnumerator FadeFromBlack()
    {
        if (transitionPanel == null) yield break;

        yield return SteppedFade(1f, 0f);
    }

    private IEnumerator SteppedFade(float from, float to)
    {
        transitionPanel.blocksRaycasts = true;

        int steps = Mathf.Max(1, fadeSteps);
        float stepTime = fadeDuration / steps;

        for (int i = 1; i <= steps; i++)
        {
            transitionPanel.alpha = Mathf.Lerp(from, to, i / (float)steps);
            yield return new WaitForSecondsRealtime(stepTime);
        }

        transitionPanel.alpha = to;
        transitionPanel.blocksRaycasts = to >= 1f;
    }
}