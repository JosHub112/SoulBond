using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("Requires an AudioSource component for Music")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("Requires an AudioSource component for SFX")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Tracks (OST)")]
    public AudioClip overworldOST;
    public AudioClip battleOST;

    [Header("Global SFX")]
    public AudioClip hitSFX;
    public AudioClip hazardSpawnSFX;
    public AudioClip soulSpawnSFX;

    private void Awake()
    {
        // Singleton pattern to ensure only one AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start with Overworld music by default
        PlayMusic(overworldOST);
    }

    // --- MODULAR METHODS ---

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            // PlayOneShot allows multiple SFX to overlap without cutting each other off
            sfxSource.PlayOneShot(clip);
        }
    }

    // --- CONVENIENCE METHODS FOR GLOBAL SOUNDS ---
    public void PlayOverworldMusic() => PlayMusic(overworldOST);
    public void PlayBattleMusic() => PlayMusic(battleOST);

    public void PlayHitSFX() => PlaySFX(hitSFX);
    public void PlayHazardSpawnSFX() => PlaySFX(hazardSpawnSFX);
    public void PlaySoulSpawnSFX() => PlaySFX(soulSpawnSFX);
}