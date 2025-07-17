using UnityEngine;

/// <summary>
/// Clase singleton que administra y expone los AudioClip del juego.
/// Permite acceder centralizadamente a los clips de audio para reproducirlos desde otros managers.
/// </summary>
public class AudioClipsManager : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton para acceder globalmente al AudioClipsManager.
    /// </summary>
    public static AudioClipsManager Instance;

    [Header("UI Sounds")]
    public AudioClip clickSound;
    public AudioClip clickButtonSelectorNext;
    public AudioClip clickButtonSelectorBack;
    public AudioClip clickButtonVolume;
    public AudioClip StartGame;
    public AudioClip EndGame;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip prelevelAbAdMusic;
    public AudioClip prelevelFEMusic;
    public AudioClip levelAbAdMusic;
    public AudioClip levelFEMusic;
    public AudioClip tutorialMusic;

    [Header("Ostacle Sounds")]
    public AudioClip ObstacleCarCollision;
    public AudioClip ObstacleCarSpawn;

    [Header("Reward Sounds")]
    public AudioClip RewardCoinCollision;
    public AudioClip RewardGemCollsion;

    [Header("Misc Sounds")]
    public AudioClip clickButtonVolumeLevelSFX;
    public AudioClip clickButtonVolumeLevelSFX2;
    public AudioClip TutorialSound;

    private void Awake()
    {
        // Configuración del singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
