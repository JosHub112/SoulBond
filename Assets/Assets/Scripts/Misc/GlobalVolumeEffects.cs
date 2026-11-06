using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum VolumeMood
{
    Overworld,
    Battle,
    EnemyTurn,
    Victory,
}

/// <summary>
/// A look for the whole screen. The GlobalVolumeEffects blends smoothly between these.
/// </summary>
[System.Serializable]
public class VolumePreset
{
    [Header("Bloom")]
    [Min(0f)] public float bloomIntensity = 0.2f;
    [Min(0f)] public float bloomThreshold = 0.95f;
    [Range(0f, 1f)] public float bloomScatter = 0.5f;
    public Color bloomTint = Color.white;

    [Header("Vignette")]
    [Range(0f, 1f)] public float vignetteIntensity = 0.3f;
    [Range(0.01f, 1f)] public float vignetteSmoothness = 0.4f;
    public Color vignetteColor = Color.black;

    [Header("Lens / Grain")]
    [Range(0f, 1f)] public float chromaticAberration;
    [Range(-1f, 1f)] public float lensDistortion;
    [Range(0f, 1f)] public float filmGrain = 0.05f;

    [Header("Color Grade")]
    public float postExposure = -0.2f; // Slightly dimmed for night comfort
    [Range(-100f, 100f)] public float contrast = 5f;
    [Range(-100f, 100f)] public float saturation = -5f; // Muted saturation to avoid eye fatigue
    [Range(-180f, 180f)] public float hueShift;
    public Color colorFilter = Color.white;

    [Header("Split Toning (grey = neutral)")]
    public Color shadowsTint = Color.gray;
    public Color highlightsTint = Color.gray;
    [Range(-100f, 100f)] public float splitBalance;

    [Header("White Balance")]
    [Range(-100f, 100f)] public float temperature = -5f; // Slightly warmer for night viewing
    [Range(-100f, 100f)] public float tint;

    [Header("Ambient Motion")]
    [Tooltip("How strongly bloom and vignette pulse on the beat.")]
    [Min(0f)] public float beatStrength = 0.3f;
    [Tooltip("How strongly the hue slowly shimmers.")]
    [Min(0f)] public float shimmer = 0.2f;

    /// <summary>Moves every value toward 'to'. t = 1 copies it outright.</summary>
    public void LerpToward(VolumePreset to, float t)
    {
        bloomIntensity = Mathf.Lerp(bloomIntensity, to.bloomIntensity, t);
        bloomThreshold = Mathf.Lerp(bloomThreshold, to.bloomThreshold, t);
        bloomScatter = Mathf.Lerp(bloomScatter, to.bloomScatter, t);
        bloomTint = Color.Lerp(bloomTint, to.bloomTint, t);

        vignetteIntensity = Mathf.Lerp(vignetteIntensity, to.vignetteIntensity, t);
        vignetteSmoothness = Mathf.Lerp(vignetteSmoothness, to.vignetteSmoothness, t);
        vignetteColor = Color.Lerp(vignetteColor, to.vignetteColor, t);

        chromaticAberration = Mathf.Lerp(chromaticAberration, to.chromaticAberration, t);
        lensDistortion = Mathf.Lerp(lensDistortion, to.lensDistortion, t);
        filmGrain = Mathf.Lerp(filmGrain, to.filmGrain, t);

        postExposure = Mathf.Lerp(postExposure, to.postExposure, t);
        contrast = Mathf.Lerp(contrast, to.contrast, t);
        saturation = Mathf.Lerp(saturation, to.saturation, t);
        hueShift = Mathf.Lerp(hueShift, to.hueShift, t);
        colorFilter = Color.Lerp(colorFilter, to.colorFilter, t);

        shadowsTint = Color.Lerp(shadowsTint, to.shadowsTint, t);
        highlightsTint = Color.Lerp(highlightsTint, to.highlightsTint, t);
        splitBalance = Mathf.Lerp(splitBalance, to.splitBalance, t);

        temperature = Mathf.Lerp(temperature, to.temperature, t);
        tint = Mathf.Lerp(tint, to.tint, t);

        beatStrength = Mathf.Lerp(beatStrength, to.beatStrength, t);
        shimmer = Mathf.Lerp(shimmer, to.shimmer, t);
    }

    // ---------------------------------------------------------------------
    // Starting looks (tweak them in the inspector)
    // ---------------------------------------------------------------------

    /// <summary>Light, clean, slightly glowy.</summary>
    public static VolumePreset DefaultOverworld()
    {
        return new VolumePreset();
    }

    /// <summary>Dark purple, glowing pink highlights, pulsing on the beat.</summary>
    public static VolumePreset DefaultBattle()
    {
        return new VolumePreset
        {
            bloomIntensity = 0.4f,
            bloomThreshold = 0.85f,
            bloomScatter = 0.5f,
            bloomTint = new Color(0.9f, 0.8f, 0.9f),

            vignetteIntensity = 0.35f,
            vignetteSmoothness = 0.5f,
            vignetteColor = new Color(0.08f, 0.01f, 0.12f),

            chromaticAberration = 0.04f,
            lensDistortion = -0.02f,
            filmGrain = 0.08f,

            postExposure = -0.3f,
            contrast = 15f,
            saturation = -10f,
            hueShift = 0f,
            colorFilter = new Color(0.9f, 0.85f, 0.95f),

            shadowsTint = new Color(0.25f, 0.18f, 0.45f),
            highlightsTint = new Color(0.65f, 0.45f, 0.7f),
            splitBalance = -5f,

            temperature = -10f,
            tint = 10f,

            beatStrength = 0.3f,
            shimmer = 0.3f,
        };
    }

    /// <summary>More crimson, more aberration, faster pulse. Used while dodging.</summary>
    public static VolumePreset DefaultEnemyTurn()
    {
        return new VolumePreset
        {
            bloomIntensity = 0.5f,
            bloomThreshold = 0.8f,
            bloomScatter = 0.55f,
            bloomTint = new Color(0.95f, 0.75f, 0.8f),

            vignetteIntensity = 0.45f,
            vignetteSmoothness = 0.5f,
            vignetteColor = new Color(0.18f, 0.01f, 0.05f),

            chromaticAberration = 0.08f,
            lensDistortion = -0.05f,
            filmGrain = 0.12f,

            postExposure = -0.4f,
            contrast = 20f,
            saturation = -15f,
            hueShift = 0f,
            colorFilter = new Color(0.95f, 0.85f, 0.9f),

            shadowsTint = new Color(0.35f, 0.15f, 0.3f),
            highlightsTint = new Color(0.8f, 0.35f, 0.45f),
            splitBalance = -10f,

            temperature = -5f,
            tint = 15f,

            beatStrength = 0.4f,
            shimmer = 0.4f,
        };
    }

    /// <summary>Bright, warm and glowing.</summary>
    public static VolumePreset DefaultVictory()
    {
        return new VolumePreset
        {
            bloomIntensity = 0.8f,
            bloomThreshold = 0.75f,
            bloomScatter = 0.6f,
            bloomTint = new Color(1f, 0.95f, 0.8f),

            vignetteIntensity = 0.2f,
            vignetteSmoothness = 0.4f,
            vignetteColor = Color.black,

            chromaticAberration = 0.02f,
            lensDistortion = 0.02f,
            filmGrain = 0.03f,

            postExposure = -0.1f, // Kept controlled instead of painfully bright (+0.5)
            contrast = 8f,
            saturation = 5f,
            hueShift = 0f,
            colorFilter = new Color(1f, 0.98f, 0.92f),

            shadowsTint = Color.gray,
            highlightsTint = new Color(0.9f, 0.8f, 0.5f),
            splitBalance = 10f,

            temperature = 5f,
            tint = -2f,

            beatStrength = 0.2f,
            shimmer = 0.2f,
        };
    }
}

/// <summary>
/// Deltarune-style post-processing for the whole game. Add it to your Global Volume (URP).
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Volume))]
public class GlobalVolumeEffects : MonoBehaviour
{
    public static GlobalVolumeEffects Instance { get; private set; }

    [Header("Mood Presets")]
    [SerializeField] private VolumePreset overworld = VolumePreset.DefaultOverworld();
    [SerializeField] private VolumePreset battle = VolumePreset.DefaultBattle();
    [SerializeField] private VolumePreset enemyTurn = VolumePreset.DefaultEnemyTurn();
    [SerializeField] private VolumePreset victory = VolumePreset.DefaultVictory();

    [Header("Mood Blending")]
    [SerializeField] private float moodBlendSpeed = 3f;

    [Header("Beat Pulse")]
    [SerializeField] private float beatsPerMinute = 120f;
    [SerializeField] private float beatBloom = 0.1f; // Subdued beat flashing

    [Header("Low Health Heartbeat")]
    [SerializeField] private float heartbeatRate = 1.4f;

    [Header("Impulse Decay (higher = shorter)")]
    [SerializeField] private float hitDecay = 3f;
    [SerializeField] private float impactDecay = 6f;
    [SerializeField] private float pulseDecay = 2.5f;
    [SerializeField] private float blockDecay = 4f;
    [SerializeField] private float glitchDecay = 5f;
    [SerializeField] private float flashDecay = 7f;
    [SerializeField] private float drainDecay = 1.2f;
    [SerializeField] private float warpDecay = 1.5f;

    [Header("Impulse Colours (Softened)")]
    [SerializeField] private Color hitColor = new Color(0.7f, 0.15f, 0.18f);
    [SerializeField] private Color blockColor = new Color(0.2f, 0.6f, 0.7f);
    [SerializeField] private Color pulseColor = new Color(0.7f, 0.3f, 0.6f);

    [Header("Glitch")]
    [SerializeField] private float glitchFps = 24f;

    // Volume overrides we drive
    private Volume volume;
    private Bloom bloom;
    private Vignette vignette;
    private ChromaticAberration chromatic;
    private LensDistortion lens;
    private FilmGrain grain;
    private ColorAdjustments colorAdjust;
    private SplitToning splitToning;
    private WhiteBalance whiteBalance;

    // Mood
    private VolumePreset current;
    private VolumePreset target;

    // Impulses
    private float hit;
    private float impact;
    private float pulse;
    private float block;
    private float glitch;
    private float flash;
    private float drain;
    private float warp;

    // Sustained states
    private float danger;
    private float dangerTarget;
    private float deathAmount;
    private bool dead;

    // Stepped glitch noise
    private float nextGlitchStep;
    private float noiseAberration;
    private float noiseLens;
    private float noiseHue;

    private bool ready;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        volume = GetComponent<Volume>();
        if (volume.sharedProfile == null)
        {
            volume.sharedProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        }

        VolumeProfile profile = volume.profile;

        bloom = Prepare<Bloom>(profile);
        vignette = Prepare<Vignette>(profile);
        chromatic = Prepare<ChromaticAberration>(profile);
        lens = Prepare<LensDistortion>(profile);
        grain = Prepare<FilmGrain>(profile);
        colorAdjust = Prepare<ColorAdjustments>(profile);
        splitToning = Prepare<SplitToning>(profile);
        whiteBalance = Prepare<WhiteBalance>(profile);

        current = new VolumePreset();
        target = overworld;
        current.LerpToward(target, 1f);

        ready = true;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void LateUpdate()
    {
        if (!ready) return;

        UpdateState(Time.unscaledDeltaTime);
        ApplyToVolume();
    }

    public void SetMood(VolumeMood mood, bool instant = false)
    {
        target = PresetFor(mood);

        if (!instant) return;

        current.LerpToward(target, 1f);
        ClearImpulses();
    }

    public void PlayHit(float strength = 1f)
    {
        hit = Mathf.Max(hit, strength * 0.7f);
        flash = Mathf.Max(flash, 0.2f * strength);
        glitch = Mathf.Max(glitch, 0.15f * strength);
    }

    public void PlayImpact(float strength = 1f)
    {
        impact = Mathf.Max(impact, strength * 0.7f);
        flash = Mathf.Max(flash, 0.15f * strength);
    }

    public void PlayPulse(float strength = 1f)
    {
        pulse = Mathf.Max(pulse, strength * 0.8f);
    }

    public void PlaySoulSummon()
    {
        pulse = 0.8f;
        flash = Mathf.Max(flash, 0.1f);
        glitch = Mathf.Max(glitch, 0.1f);
    }

    public void PlayBlock()
    {
        block = 1f;
        pulse = Mathf.Max(pulse, 0.3f);
    }

    public void PlayGlitch(float strength = 1f)
    {
        glitch = Mathf.Max(glitch, strength * 0.6f);
    }

    public void PlayFlash(float strength = 1f)
    {
        flash = Mathf.Max(flash, strength * 0.5f); // Half intensity flash to protect eyes
    }

    public void PlayBattleStart()
    {
        warp = 0.6f;
        flash = Mathf.Max(flash, 0.3f);
        glitch = Mathf.Max(glitch, 0.2f);
        pulse = 0.8f;
    }

    public void PlayVictory()
    {
        SetMood(VolumeMood.Victory);
        flash = 0.5f;
        pulse = 0.8f;
    }

    public void PlayFlee()
    {
        drain = 1f;
        warp = Mathf.Max(warp, 0.3f);
    }

    public void PlayDeath()
    {
        dead = true;
        hit = 0.8f;
        glitch = 0.6f;
        flash = 0.4f;
    }

    public void SetDanger(float amount)
    {
        dangerTarget = Mathf.Clamp01(amount);
    }

    public void ResetEffects()
    {
        dead = false;
        deathAmount = 0f;
        danger = 0f;
        dangerTarget = 0f;
        SetMood(VolumeMood.Overworld, true);
    }

    private void UpdateState(float dt)
    {
        current.LerpToward(target, 1f - Mathf.Exp(-moodBlendSpeed * dt));

        hit = Decay(hit, hitDecay, dt);
        impact = Decay(impact, impactDecay, dt);
        pulse = Decay(pulse, pulseDecay, dt);
        block = Decay(block, blockDecay, dt);
        glitch = Decay(glitch, glitchDecay, dt);
        flash = Decay(flash, flashDecay, dt);
        drain = Decay(drain, drainDecay, dt);
        warp = Decay(warp, warpDecay, dt);

        danger = Mathf.MoveTowards(danger, dangerTarget, dt * 1.5f);
        deathAmount = Mathf.MoveTowards(deathAmount, dead ? 1f : 0f, dt * 0.8f);

        if (glitch > 0f && Time.unscaledTime >= nextGlitchStep)
        {
            nextGlitchStep = Time.unscaledTime + 1f / Mathf.Max(1f, glitchFps);
            noiseAberration = Random.value * 0.5f; // Muted glitch severity
            noiseLens = Random.Range(-0.5f, 0.5f);
            noiseHue = Random.Range(-0.5f, 0.5f);
        }
    }

    private void ApplyToVolume()
    {
        VolumePreset b = current;
        float time = Time.unscaledTime;

        float beat = b.beatStrength > 0f ? BeatEnvelope(time) * b.beatStrength : 0f;
        float heart = danger > 0.001f ? HeartbeatEnvelope((time * heartbeatRate) % 1f) * danger : 0f;
        float shimmerWave = Mathf.Sin(time * 0.7f) * b.shimmer;

        float red = Mathf.Clamp01(hit + heart * 0.6f + deathAmount);

        // Bloom (softened multipliers)
        bloom.intensity.value = b.bloomIntensity + beat * beatBloom + impact * 0.6f + pulse * 0.5f + flash * 0.8f + warp * 0.2f;
        bloom.threshold.value = b.bloomThreshold;
        bloom.scatter.value = b.bloomScatter;

        Color bloomTint = Color.Lerp(b.bloomTint, pulseColor, pulse * 0.4f);
        bloomTint = Color.Lerp(bloomTint, blockColor, block * 0.8f);
        bloomTint = Color.Lerp(bloomTint, Color.white, Mathf.Clamp01(impact + flash) * 0.5f);
        bloom.tint.value = bloomTint;

        // Vignette
        vignette.intensity.value = b.vignetteIntensity
            + beat * 0.02f
            + hit * 0.2f
            + heart * 0.25f
            + deathAmount * 0.35f
            + block * 0.08f
            + warp * 0.3f;
        vignette.smoothness.value = b.vignetteSmoothness;

        Color vignetteColor = Color.Lerp(b.vignetteColor, hitColor, red);
        vignetteColor = Color.Lerp(vignetteColor, blockColor, block * 0.4f);
        vignette.color.value = vignetteColor;

        // Chromatic aberration (dampened to avoid aggressive eye-straining pixel separation)
        chromatic.intensity.value = (b.chromaticAberration
            + hit * 0.3f
            + impact * 0.2f
            + pulse * 0.1f
            + heart * 0.1f
            + deathAmount * 0.25f
            + warp * 0.3f
            + glitch * noiseAberration) * 0.6f;

        // Lens distortion
        lens.intensity.value = b.lensDistortion
            - hit * 0.15f
            + impact * 0.1f
            + pulse * 0.05f
            - warp * 0.3f
            - deathAmount * 0.15f
            + glitch * noiseLens * 0.2f;

        // Film grain
        grain.intensity.value = b.filmGrain
            + glitch * 0.2f
            + heart * 0.1f
            + deathAmount * 0.2f;

        // Color adjustments (exposures and contrasts clamped to prevent whiteouts)
        colorAdjust.postExposure.value = Mathf.Clamp(b.postExposure
            + beat * 0.03f
            + flash * 0.5f
            + impact * 0.2f
            + hit * 0.1f
            - deathAmount * 0.5f
            - warp * 0.5f, -1.5f, 0.5f);

        colorAdjust.contrast.value = b.contrast
            + impact * 8f
            + hit * 6f
            + heart * 5f
            + deathAmount * 15f;

        colorAdjust.saturation.value = b.saturation
            + pulse * 10f
            - hit * 10f
            - drain * 40f
            - deathAmount * 60f;

        colorAdjust.hueShift.value = b.hueShift
            + shimmerWave * 4f
            + pulse * 10f
            + glitch * noiseHue * 20f;

        Color filter = Color.Lerp(b.colorFilter, hitColor, hit * 0.4f);
        filter = Color.Lerp(filter, blockColor, block * 0.3f);
        filter = Color.Lerp(filter, Color.white, Mathf.Clamp01(flash) * 0.5f);
        colorAdjust.colorFilter.value = filter;

        // Split toning
        splitToning.shadows.value = Color.Lerp(b.shadowsTint, hitColor, red * 0.3f);
        splitToning.highlights.value = Color.Lerp(b.highlightsTint, pulseColor, pulse * 0.3f);
        splitToning.balance.value = b.splitBalance;

        // White balance
        whiteBalance.temperature.value = b.temperature - block * 15f + hit * 10f;
        whiteBalance.tint.value = b.tint + pulse * 10f + glitch * noiseHue * 15f;
    }

    private VolumePreset PresetFor(VolumeMood mood)
    {
        switch (mood)
        {
            case VolumeMood.Battle: return battle;
            case VolumeMood.EnemyTurn: return enemyTurn;
            case VolumeMood.Victory: return victory;
            default: return overworld;
        }
    }

    private void ClearImpulses()
    {
        hit = 0f;
        impact = 0f;
        pulse = 0f;
        block = 0f;
        glitch = 0f;
        flash = 0f;
        drain = 0f;
        warp = 0f;
    }

    private static float Decay(float value, float rate, float dt)
    {
        if (value <= 0.001f) return 0f;
        return value * Mathf.Exp(-rate * dt);
    }

    private float BeatEnvelope(float time)
    {
        float phase = (time * beatsPerMinute / 60f) % 1f;
        return Mathf.Pow(1f - phase, 3f);
    }

    private static float HeartbeatEnvelope(float phase)
    {
        float lub = Mathf.Clamp01(1f - Mathf.Abs(phase - 0.05f) / 0.08f);
        float dub = Mathf.Clamp01(1f - Mathf.Abs(phase - 0.27f) / 0.08f) * 0.65f;
        return Mathf.Max(lub, dub);
    }

    private static T Prepare<T>(VolumeProfile profile) where T : VolumeComponent, new()
    {
        if (!profile.TryGet(out T component))
        {
            component = profile.Add<T>(true);
        }

        component.active = true;

        for (int i = 0; i < component.parameters.Count; i++)
        {
            component.parameters[i].overrideState = true;
        }

        return component;
    }

    [ContextMenu("Test/Hit")] private void TestHit() => PlayHit();
    [ContextMenu("Test/Impact")] private void TestImpact() => PlayImpact();
    [ContextMenu("Test/Glitch")] private void TestGlitch() => PlayGlitch();
    [ContextMenu("Test/Battle Start")] private void TestBattleStart() => PlayBattleStart();
    [ContextMenu("Test/Mood Battle")] private void TestMoodBattle() => SetMood(VolumeMood.Battle);
    [ContextMenu("Test/Mood Enemy Turn")] private void TestMoodEnemy() => SetMood(VolumeMood.EnemyTurn);
    [ContextMenu("Test/Victory")] private void TestVictory() => PlayVictory();
    [ContextMenu("Test/Danger On")] private void TestDangerOn() => SetDanger(1f);
    [ContextMenu("Test/Death")] private void TestDeath() => PlayDeath();
    [ContextMenu("Test/Reset")] private void TestReset() => ResetEffects();
}