using System.Collections;

using TMPro;

using UnityEngine;

using UnityEngine.Events;

using UnityEngine.EventSystems;

using UnityEngine.SceneManagement;

using UnityEngine.UI;





public class DeathScreen : MonoBehaviour

{

    [Header("References")]

    [SerializeField] private Image panel;

    [SerializeField] private TMP_Text titleText;

    [SerializeField] private Button restartButton;

    [SerializeField] private TMP_Text restartText;



    [Header("Pixel Look")]

    [Tooltip("UI units per pixel step. Bigger = chunkier shake.")]

    [SerializeField] private float pixelSize = 3f;

    [Tooltip("How often shake / scale / bob update per second. Lower = more retro.")]

    [SerializeField] private float animationFps = 12f;

    [Tooltip("How much scale changes per animation tick when settling.")]

    [SerializeField] private float scaleStepPerTick = 0.1f;



    [Header("Intro")]

    [SerializeField] private float panelFadeDuration = 0.8f;

    [SerializeField] private int panelFadeSteps = 6;

    [SerializeField] private float delayBeforeTitle = 0.4f;

    [SerializeField] private float letterInterval = 0.12f;

    [SerializeField] private float titleSlamScale = 1.4f;

    [SerializeField] private float delayBeforeRestart = 0.7f;

    [SerializeField] private int restartFlickerCount = 4;



    [Header("Shake (in pixel steps)")]

    [SerializeField] private float titleIdleShake = 1f;

    [SerializeField] private float titleLetterShake = 2f;

    [SerializeField] private float titleSlamShake = 6f;

    [SerializeField] private float restartHoverShake = 1f;

    [SerializeField] private float restartHoverPop = 3f;

    [SerializeField] private float restartClickShake = 5f;

    [SerializeField] private float shakeDecayPerSecond = 12f;



    [Header("Colors")]

    [SerializeField] private Color titleColor = Color.white;

    [SerializeField] private Color slamFlashColor = new Color(0.85f, 0.1f, 0.1f);

    [SerializeField] private Color restartNormalColor = new Color(0.6f, 0.6f, 0.6f);

    [SerializeField] private Color restartHoverColor = Color.white;



    [Header("Hover / Click")]

    [SerializeField] private float hoverScale = 1.15f;

    [SerializeField] private float clickScale = 0.8f;

    [SerializeField] private bool showArrowsOnHover = true;

    [SerializeField] private float restartDelayAfterClick = 0.5f;

    [SerializeField] private int bobEveryTicks = 6;



    [Header("Restart")]

    [Tooltip("Leave empty to reload the current scene.")]

    [SerializeField] private string sceneToLoad = "";



    // --- runtime state ---

    private JitterText titleJitter, restartJitter;

    private RectTransform titleRect, restartRect;

    private Vector2 restartBasePos;

    private string restartBaseString;



    private float titleBurst, restartBurst;

    private float titleScale = 1f, restartScale = 1f, restartScaleTarget = 1f;

    private int titleFlashTicks;

    private int bobTickCounter;

    private bool bobUp;

    private float tickTimer;



    private bool refsOk, isShowing, canInteract, isRestarting, hovered;



    // ------------------------------------------------------------------ setup



    private void Awake()

    {

        if (titleText == null || restartButton == null || restartText == null)

        {

            Debug.LogError("DeathScreen: assign Title Text, Restart Button and Restart Text in the Inspector.", this);

            return;

        }



        refsOk = true;



        titleRect = titleText.rectTransform;

        restartRect = restartText.rectTransform;

        restartBasePos = restartRect.anchoredPosition;

        restartBaseString = restartText.text;



        titleJitter = new JitterText(titleText);

        restartJitter = new JitterText(restartText);



        // Hover / keyboard-select events

        EventTrigger trigger = restartButton.gameObject.GetComponent<EventTrigger>();

        if (trigger == null) trigger = restartButton.gameObject.AddComponent<EventTrigger>();



        AddTrigger(trigger, EventTriggerType.PointerEnter, _ => SetHovered(true));

        AddTrigger(trigger, EventTriggerType.PointerExit, _ => SetHovered(false));

        AddTrigger(trigger, EventTriggerType.Select, _ => SetHovered(true));

        AddTrigger(trigger, EventTriggerType.Deselect, _ => SetHovered(false));



        restartButton.onClick.AddListener(OnRestartClicked);

    }



    private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> action)

    {

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };

        entry.callback.AddListener(action);

        trigger.triggers.Add(entry);

    }



    // ------------------------------------------------------------------ public API



    public void Show()

    {

        if (isShowing) return;



        gameObject.SetActive(true); // runs Awake the first time

        if (!refsOk) return;



        isShowing = true;

        ResetVisuals();



        // Play Death OST when death screen opens

        AudioManager.Instance?.PlayDeathMusic();



        StartCoroutine(IntroRoutine());

    }



    private void ResetVisuals()

    {

        SetPanelAlpha(0f);



        titleText.maxVisibleCharacters = 0;

        titleText.color = titleColor;



        restartText.text = restartBaseString;

        restartText.maxVisibleCharacters = 0;

        restartText.color = restartNormalColor;



        titleScale = restartScale = restartScaleTarget = 1f;

        titleRect.localScale = Vector3.one;

        restartRect.localScale = Vector3.one;

        restartRect.anchoredPosition = restartBasePos;



        titleBurst = restartBurst = 0f;

        titleFlashTicks = 0;

        hovered = false;

        canInteract = false;

        isRestarting = false;

        restartButton.interactable = false;



        titleJitter.Clear();

        restartJitter.Clear();

    }



    // ------------------------------------------------------------------ intro



    private IEnumerator IntroRoutine()

    {

        // 1. Stepped (chunky) fade to black

        int steps = Mathf.Max(1, panelFadeSteps);

        for (int i = 1; i <= steps; i++)

        {

            SetPanelAlpha((float)i / steps);

            yield return new WaitForSecondsRealtime(panelFadeDuration / steps);

        }



        yield return new WaitForSecondsRealtime(delayBeforeTitle);



        // 2. Type out the title letter by letter, each letter gives a little shake

        titleText.ForceMeshUpdate();

        int total = titleText.textInfo.characterCount;

        for (int i = 1; i <= total; i++)

        {

            titleText.maxVisibleCharacters = i;

            titleBurst = Mathf.Max(titleBurst, titleLetterShake);

            yield return new WaitForSecondsRealtime(letterInterval);

        }



        // 3. Slam: big scale, red/white flicker, heavy shake

        titleScale = titleSlamScale;

        titleFlashTicks = 6;

        titleBurst = titleSlamShake;



        yield return new WaitForSecondsRealtime(delayBeforeRestart);



        // 4. Restart text flickers in

        for (int i = 0; i < restartFlickerCount; i++)

        {

            restartText.maxVisibleCharacters = 99999;

            yield return new WaitForSecondsRealtime(0.06f);

            restartText.maxVisibleCharacters = 0;

            yield return new WaitForSecondsRealtime(0.06f);

        }

        restartText.maxVisibleCharacters = 99999;



        // 5. Ready

        canInteract = true;

        restartButton.interactable = true;

        if (EventSystem.current != null)

            EventSystem.current.SetSelectedGameObject(restartButton.gameObject); // keyboard / gamepad support

    }



    // ------------------------------------------------------------------ hover / click



    private void SetHovered(bool value)

    {

        if (!canInteract || isRestarting || hovered == value) return;



        hovered = value;

        restartScaleTarget = value ? hoverScale : 1f;

        restartText.color = value ? restartHoverColor : restartNormalColor;

        restartText.text = (value && showArrowsOnHover) ? $"> {restartBaseString} <" : restartBaseString;



        if (value)

        {

            restartBurst = Mathf.Max(restartBurst, restartHoverPop);

            // Play Glitch SFX on hover

            AudioManager.Instance?.PlayGlitchSFX();

        }

    }



    private void OnRestartClicked()

    {

        if (!canInteract || isRestarting) return;



        isRestarting = true;

        canInteract = false;

        StartCoroutine(RestartRoutine());

    }



    private IEnumerator RestartRoutine()

    {

        // Click punch: squash down, then spring back with a heavy shake

        restartScale = clickScale;

        restartScaleTarget = hoverScale;

        restartBurst = restartClickShake;

        titleBurst = Mathf.Max(titleBurst, 3f);

        titleFlashTicks = 4;



        // Switch music back to overworld when restarting!

        AudioManager.Instance?.PlayOverworldMusic();



        yield return new WaitForSecondsRealtime(restartDelayAfterClick);



        Time.timeScale = 1f;



        if (string.IsNullOrEmpty(sceneToLoad))

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        else

            SceneManager.LoadScene(sceneToLoad);

    }



    // ------------------------------------------------------------------ per-frame animation



    private void Update()

    {

        if (!isShowing) return;



        float dt = Time.unscaledDeltaTime;



        // Shake bursts fade out smoothly; the amplitude is rounded to whole pixel steps when applied

        titleBurst = Mathf.MoveTowards(titleBurst, 0f, shakeDecayPerSecond * dt);

        restartBurst = Mathf.MoveTowards(restartBurst, 0f, shakeDecayPerSecond * dt);



        // Everything else only updates a few times per second for the retro look

        float interval = 1f / Mathf.Max(1f, animationFps);

        tickTimer += dt;

        if (tickTimer >= interval)

        {

            tickTimer %= interval;

            Tick();

        }

    }



    private void Tick()

    {

        // --- Title ---

        titleJitter.Regenerate(titleIdleShake + titleBurst, pixelSize);

        titleScale = Mathf.MoveTowards(titleScale, 1f, scaleStepPerTick);

        titleRect.localScale = Vector3.one * titleScale;



        Color wantedTitleColor = (titleFlashTicks > 0 && titleFlashTicks % 2 == 1) ? slamFlashColor : titleColor;

        if (titleFlashTicks > 0) titleFlashTicks--;

        titleText.color = wantedTitleColor;



        // --- Restart ---

        float restartAmp = restartBurst + (hovered ? restartHoverShake : 0f);

        restartJitter.Regenerate(restartAmp, pixelSize);

        restartScale = Mathf.MoveTowards(restartScale, restartScaleTarget, scaleStepPerTick);

        restartRect.localScale = Vector3.one * restartScale;



        // Idle bob: hops up one pixel step every few ticks (stops while hovered)

        bobTickCounter++;

        if (bobTickCounter >= Mathf.Max(1, bobEveryTicks))

        {

            bobTickCounter = 0;

            bobUp = !bobUp;

        }

        float bobY = (!hovered && bobUp) ? pixelSize : 0f;

        restartRect.anchoredPosition = restartBasePos + new Vector2(0f, bobY);

    }



    private void LateUpdate()

    {

        if (!isShowing) return;



        // Re-apply every frame so the shake survives TMP rebuilding the mesh

        titleJitter.Apply();

        restartJitter.Apply();

    }



    private void SetPanelAlpha(float a)

    {

        if (panel == null) return;

        Color c = panel.color;

        c.a = a;

        panel.color = c;

    }



    // ------------------------------------------------------------------ per-letter shake



    /// <summary>Shakes every visible character of a TMP text independently.</summary>

    private class JitterText

    {

        private readonly TMP_Text text;

        private Vector3[] offsets = new Vector3[0];



        public JitterText(TMP_Text text) { this.text = text; }



        public void Clear() { offsets = new Vector3[0]; }



        /// <summary>Picks new random offsets, snapped to whole pixel steps.</summary>

        public void Regenerate(float amplitudePixels, float pixelSize)

        {

            int steps = Mathf.RoundToInt(amplitudePixels);

            int len = text.text.Length;

            if (offsets.Length != len) offsets = new Vector3[len];



            for (int i = 0; i < len; i++)

            {

                offsets[i] = steps <= 0

                    ? Vector3.zero

                    : new Vector3(Random.Range(-steps, steps + 1), Random.Range(-steps, steps + 1), 0f) * pixelSize;

            }

        }



        public void Apply()

        {

            text.ForceMeshUpdate();

            TMP_TextInfo info = text.textInfo;



            for (int i = 0; i < info.characterCount; i++)

            {

                if (i >= text.maxVisibleCharacters) break;

                if (i >= offsets.Length) break;



                TMP_CharacterInfo ci = info.characterInfo[i];

                if (!ci.isVisible) continue;



                Vector3[] verts = info.meshInfo[ci.materialReferenceIndex].vertices;

                int v = ci.vertexIndex;

                for (int j = 0; j < 4; j++) verts[v + j] += offsets[i];

            }



            for (int m = 0; m < info.meshInfo.Length; m++)

            {

                info.meshInfo[m].mesh.vertices = info.meshInfo[m].vertices;

                text.UpdateGeometry(info.meshInfo[m].mesh, m);

            }

        }

    }

}