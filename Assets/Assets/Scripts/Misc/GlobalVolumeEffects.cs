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
    [Min(0f)] public float bloomIntensity = 0.35f;
    [Min(0f)] public float bloomThreshold = 0.9f;
    [Range(0f, 1f)] public float bloomScatter = 0.6f;
    public Color bloomTint = Color.white;

    [Header("Vignette")]
    [Range(0f, 1f)] public float vignetteIntensity = 0.2f;
    [Range(0.01f, 1f)] public float vignetteSmoothness = 0.4f;
    public Color vignetteColor = Color.black;

    [Header("Lens / Grain")]
    [Range(0f, 1f)] public float chromaticAberration;
    [Range(-1f, 1f)] public float lensDistortion;
    [Range(0f, 1f)] public float filmGrain;

    [Header("Color Grade")]
    public float postExposure;
    [Range(-100f, 100f)] public float contrast = 8f;
    [Range(-100f, 100f)] public float saturation = 10f;
    [Range(-180f, 180f)] public float hueShift;
    public Color colorFilter = Color.white;

    [Header("Split Toning (grey = neutral)")]
    public Color shadowsTint = Color.gray;
    public Color highlightsTint = Color.gray;
    [Range(-100f, 100f)] public float splitBalance;

    [Header("White Balance")]
    [Range(-100f, 100f)] public float temperature;
    [Range(-100f, 100f)] public float tint;

    [Header("Ambient Motion")]
    [Tooltip("How strongly bloom and vignette pulse on the beat.")]
    [Min(0f)] public float beatStrength;
    [Tooltip("How strongly the hue slowly shimmers.")]
    [Min(0f)] public float shimmer;

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
            bloomIntensity = 1.1f,
            bloomThreshold = 0.75f,
            bloomScatter = 0.7f,
            bloomTint = new Color(1f, 0.85f, 1f),

            vignetteIntensity = 0.45f,
            vignetteSmoothness = 0.55f,
            vignetteColor = new Color(0.12f, 0.02f, 0.2f),

            chromaticAberration = 0.12f,
            lensDistortion = -0.05f,
            filmGrain = 0.18f,

            postExposure = -0.15f,
            contrast = 28f,
            saturation = 28f,
            hueShift = 0f,
            colorFilter = new Color(0.92f, 0.86f, 1f),

            shadowsTint = new Color(0.35f, 0.25f, 0.65f),
            highlightsTint = new Color(0.85f, 0.55f, 0.9f),
            splitBalance = -10f,

            temperature = -12f,
            tint = 15f,

            beatStrength = 1f,
            shimmer = 1f,
        };
    }

    /// <summary>More crimson, more aberration, faster pulse. Used while dodging.</summary>
    public static VolumePreset DefaultEnemyTurn()
    {
        return new VolumePreset
        {
            bloomIntensity = 1.5f,
            bloomThreshold = 0.7f,
            bloomScatter = 0.75f,
            bloomTint = new Color(1f, 0.7f, 0.85f),

            vignetteIntensity = 0.58f,
            vignetteSmoothness = 0.5f,
            vignetteColor = new Color(0.3f, 0.02f, 0.1f),

            chromaticAberration = 0.25f,
            lensDistortion = -0.12f,
            filmGrain = 0.28f,

            postExposure = -0.25f,
            contrast = 38f,
            saturation = 40f,
            hueShift = 0f,
            colorFilter = new Color(1f, 0.88f, 0.95f),

            shadowsTint = new Color(0.55f, 0.2f, 0.45f),
            highlightsTint = new Color(1f, 0.45f, 0.6f),
            splitBalance = -20f,

            temperature = 5f,
            tint = 25f,

            beatStrength = 1.6f,
            shimmer = 1.5f,
        };
    }

    /// <summary>Bright, warm and glowing.</summary>
    public static VolumePreset DefaultVictory()
    {
        return new VolumePreset
        {
            bloomIntensity = 2.2f,
            bloomThreshold = 0.6f,
            bloomScatter = 0.8f,
            bloomTint = new Color(1f, 0.95f, 0.7f),

            vignetteIntensity = 0.15f,
            vignetteSmoothness = 0.4f,
            vignetteColor = Color.black,

            chromaticAberration = 0.05f,
            lensDistortion = 0.05f,
            filmGrain = 0.05f,

            postExposure = 0.5f,
            contrast = 10f,
            saturation = 35f,
            hueShift = 0f,
            colorFilter = new Color(1f, 0.97f, 0.88f),

            shadowsTint = Color.gray,
            highlightsTint = new Color(1f, 0.85f, 0.4f),
            splitBalance = 20f,

            temperature = 15f,
            tint = -5f,

            beatStrength = 0.5f,
            shimmer = 0.5f,
        };
    }
}

/// <summary>
/// Deltarune-style post-processing for the whole game. Add it to your Global Volume (URP).
///
/// Two layers are combined every frame:
///  1. A MOOD: a base look (Overworld / Battle / EnemyTurn / Victory) the screen blends toward,
///     with a beat pulse, a slow hue shimmer and a low-health heartbeat on top.
///  2. IMPULSES: short bursts (hit, impact, pulse, block, glitch, flash, drain, warp)
///     that spike and then fade away. They can overlap freely.
///
/// Call it from anywhere with GlobalVolumeEffects.Instance?.PlayHit(), etc.
/// Missing overrides are added to a runtime copy of the profile, so the asset on disk isn't modified.
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
    [Tooltip("How fast the screen blends between moods. Higher = faster.")]
    [SerializeField] private float moodBlendSpeed = 3f;

    [Header("Beat Pulse")]
    [Tooltip("Free-running, not synced to the audio. Match it to your battle music.")]
    [SerializeField] private float beatsPerMinute = 120f;
    [SerializeField] private float beatBloom = 0.35f;

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

    [Header("Impulse Colours")]
    [SerializeField] private Color hitColor = new Color(1f, 0.1f, 0.15f);
    [SerializeField] private Color blockColor = new Color(0.3f, 0.9f, 1f);
    [SerializeField] private Color pulseColor = new Color(1f, 0.4f, 0.9f);

    [Header("Glitch")]
    [Tooltip("How many times per second the glitch jumps to a new random value.")]
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

    // Impulses (0..1, fade on their own)
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

        // volume.profile is a runtime copy, so the asset on disk stays untouched
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

    // ---------------------------------------------------------------------
    // Public API: moods
    // ---------------------------------------------------------------------

    /// <summary>
    /// Blends the screen toward a mood.
    /// instant = true snaps straight to it and clears all bursts. Use it while the screen is black.
    /// </summary>
    public void SetMood(VolumeMood mood, bool instant = false)
    {
        target = PresetFor(mood);

        if (!instant) return;

        current.LerpToward(target, 1f);
        ClearImpulses();
    }

    // ---------------------------------------------------------------------
    // Public API: bursts
    // ---------------------------------------------------------------------

    /// <summary>The player took damage: red flash, aberration, inward lens punch.</summary>
    public void PlayHit(float strength = 1f)
    {
        hit = Mathf.Max(hit, strength);
        flash = Mathf.Max(flash, 0.5f * strength);
        glitch = Mathf.Max(glitch, 0.3f * strength);
    }

    /// <summary>A strike lands: white bloom flash, outward lens punch.</summary>
    public void PlayImpact(float strength = 1f)
    {
        impact = Mathf.Max(impact, strength);
        flash = Mathf.Max(flash, 0.25f * strength);
    }

    /// <summary>Soft magenta glow with a small hue kick. Good for menus and charging.</summary>
    public void PlayPulse(float strength = 1f)
    {
        pulse = Mathf.Max(pulse, strength);
    }

    /// <summary>The soul appears.</summary>
    public void PlaySoulSummon()
    {
        pulse = 1f;
        flash = Mathf.Max(flash, 0.2f);
        glitch = Mathf.Max(glitch, 0.2f);
    }

    /// <summary>Cool cyan tint, vignette tightens.</summary>
    public void PlayBlock()
    {
        block = 1f;
        pulse = Mathf.Max(pulse, 0.4f);
    }

    /// <summary>Stepped aberration, lens and hue jumps. Spamton-style.</summary>
    public void PlayGlitch(float strength = 1f)
    {
        glitch = Mathf.Max(glitch, strength);
    }

    /// <summary>A quick white flash.</summary>
    public void PlayFlash(float strength = 1f)
    {
        flash = Mathf.Max(flash, strength);
    }

    /// <summary>Battle intro: starts dark and warped, then snaps into focus with a flash.</summary>
    public void PlayBattleStart()
    {
        warp = 1f;
        flash = Mathf.Max(flash, 0.6f);
        glitch = Mathf.Max(glitch, 0.4f);
        pulse = 1f;
    }

    /// <summary>Bright, warm glow.</summary>
    public void PlayVictory()
    {
        SetMood(VolumeMood.Victory);
        flash = 1f;
        pulse = 1f;
    }

    /// <summary>Colour drains out with a quick warp.</summary>
    public void PlayFlee()
    {
        drain = 1f;
        warp = Mathf.Max(warp, 0.5f);
    }

    /// <summary>Red, desaturated, glitching. Stays until ResetEffects().</summary>
    public void PlayDeath()
    {
        dead = true;
        hit = 1f;
        glitch = 1f;
        flash = 0.8f;
    }

    // ---------------------------------------------------------------------
    // Public API: sustained
    // ---------------------------------------------------------------------

    /// <summary>0 = fine, 1 = about to die. Drives the red heartbeat vignette.</summary>
    public void SetDanger(float amount)
    {
        dangerTarget = Mathf.Clamp01(amount);
    }

    /// <summary>Clears death, danger and all bursts, and snaps back to the overworld look.</summary>
    public void ResetEffects()
    {
        dead = false;
        deathAmount = 0f;
        danger = 0f;
        dangerTarget = 0f;
        SetMood(VolumeMood.Overworld, true);
    }

    // ---------------------------------------------------------------------
    // Update
    // ---------------------------------------------------------------------

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

        // The glitch jumps in steps instead of changing smoothly every frame
        if (glitch > 0f && Time.unscaledTime >= nextGlitchStep)
        {
            nextGlitchStep = Time.unscaledTime + 1f / Mathf.Max(1f, glitchFps);
            noiseAberration = Random.value;
            noiseLens = Random.Range(-1f, 1f);
            noiseHue = Random.Range(-1f, 1f);
        }
    }

    private void ApplyToVolume()
    {
        VolumePreset b = current;
        float time = Time.unscaledTime;

        float beat = b.beatStrength > 0f ? BeatEnvelope(time) * b.beatStrength : 0f;
        float heart = danger > 0.001f ? HeartbeatEnvelope((time * heartbeatRate) % 1f) * danger : 0f;
        float shimmerWave = Mathf.Sin(time * 0.7f) * b.shimmer;

        // How "red" the screen is from damage, low health or death
        float red = Mathf.Clamp01(hit + heart * 0.8f + deathAmount);

        // Bloom
        bloom.intensity.value = b.bloomIntensity + beat * beatBloom + impact * 2f + pulse * 1.5f + flash * 3f + warp * 0.5f;
        bloom.threshold.value = b.bloomThreshold;
        bloom.scatter.value = b.bloomScatter;

        Color bloomTint = Color.Lerp(b.bloomTint, pulseColor, pulse * 0.6f);
        bloomTint = Color.Lerp(bloomTint, blockColor, block);
        bloomTint = Color.Lerp(bloomTint, Color.white, Mathf.Clamp01(impact + flash));
        bloom.tint.value = bloomTint;

        // Vignette
        vignette.intensity.value = b.vignetteIntensity
            + beat * 0.04f
            + hit * 0.3f
            + heart * 0.35f
            + deathAmount * 0.45f
            + block * 0.12f
            + warp * 0.5f;
        vignette.smoothness.value = b.vignetteSmoothness;

        Color vignetteColor = Color.Lerp(b.vignetteColor, hitColor, red);
        vignetteColor = Color.Lerp(vignetteColor, blockColor, block * 0.6f);
        vignette.color.value = vignetteColor;

        // Chromatic aberration
        chromatic.intensity.value = b.chromaticAberration
            + hit * 0.8f
            + impact * 0.5f
            + pulse * 0.25f
            + heart * 0.2f
            + deathAmount * 0.5f
            + warp * 0.7f
            + glitch * noiseAberration;

        // Lens distortion: hit punches in, impact bulges out, warp squeezes in
        lens.intensity.value = b.lensDistortion
            - hit * 0.35f
            + impact * 0.25f
            + pulse * 0.1f
            - warp * 0.6f
            - deathAmount * 0.25f
            + glitch * noiseLens * 0.4f;

        // Film grain
        grain.intensity.value = b.filmGrain
            + glitch * 0.5f
            + heart * 0.2f
            + deathAmount * 0.3f;

        // Color adjustments
        colorAdjust.postExposure.value = b.postExposure
            + beat * 0.08f
            + flash * 2.5f
            + impact * 0.7f
            + hit * 0.5f
            - deathAmount * 0.9f
            - warp * 1.5f;

        colorAdjust.contrast.value = b.contrast
            + impact * 20f
            + hit * 15f
            + heart * 10f
            + deathAmount * 30f;

        colorAdjust.saturation.value = b.saturation
            + pulse * 25f
            - hit * 25f
            - drain * 70f
            - deathAmount * 100f;

        colorAdjust.hueShift.value = b.hueShift
            + shimmerWave * 8f
            + pulse * 20f
            + glitch * noiseHue * 60f;

        Color filter = Color.Lerp(b.colorFilter, hitColor, hit * 0.7f);
        filter = Color.Lerp(filter, blockColor, block * 0.5f);
        filter = Color.Lerp(filter, Color.white, Mathf.Clamp01(flash));
        colorAdjust.colorFilter.value = filter;

        // Split toning
        splitToning.shadows.value = Color.Lerp(b.shadowsTint, hitColor, red * 0.5f);
        splitToning.highlights.value = Color.Lerp(b.highlightsTint, pulseColor, pulse * 0.5f);
        splitToning.balance.value = b.splitBalance;

        // White balance: block cools the screen, hit warms it
        whiteBalance.temperature.value = b.temperature - block * 30f + hit * 20f;
        whiteBalance.tint.value = b.tint + pulse * 20f + glitch * noiseHue * 30f;
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

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

    /// <summary>Sharp spike on every beat that eases out.</summary>
    private float BeatEnvelope(float time)
    {
        float phase = (time * beatsPerMinute / 60f) % 1f;
        return Mathf.Pow(1f - phase, 3f);
    }

    /// <summary>Two quick thumps ("lub-dub") followed by a rest.</summary>
    private static float HeartbeatEnvelope(float phase)
    {
        float lub = Mathf.Clamp01(1f - Mathf.Abs(phase - 0.05f) / 0.08f);
        float dub = Mathf.Clamp01(1f - Mathf.Abs(phase - 0.27f) / 0.08f) * 0.65f;
        return Mathf.Max(lub, dub);
    }

    /// <summary>Finds the override on the profile, adds it if missing, and switches every parameter on.</summary>
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

    // ---------------------------------------------------------------------
    // Editor testing: right-click the component header while in Play mode
    // ---------------------------------------------------------------------

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