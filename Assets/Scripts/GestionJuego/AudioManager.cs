using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Instancia singleton para acceder globalmente al AudioManager
    public static AudioManager Instance;

    // Estado para mutear o no sonidos de enemigos
    private bool isObstacleMuted = false;

    [Header("General AudioSources")]
    public AudioSource musicSourceGeneral;
    public AudioSource sfxSourceGeneral;

    [Header("Level-specific AudioSources")]
    public AudioSource musicSourceLevel;
    public AudioSource sfxSourceLevel;
    public AudioSource sfxSourceLevelObstacle;

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

    /// <summary>
    /// Pausa todas las fuentes de audio activas (música y SFX) en general y del nivel.
    /// </summary>
    public void PauseMusic()
    {
        if ((musicSourceGeneral != null && musicSourceGeneral.isPlaying) || (musicSourceLevel != null && musicSourceLevel.isPlaying))
        {
            musicSourceGeneral.Pause();
            musicSourceLevel.Pause();
            sfxSourceGeneral.Pause();
            sfxSourceLevel.Pause();
            sfxSourceLevelObstacle.Pause();
        }
    }

    /// <summary>
    /// Reanuda la reproducción de todas las fuentes de audio
    /// </summary>
    public void UnpauseMusic()
    {
        if (musicSourceGeneral != null)
        {
            musicSourceGeneral.UnPause();
            sfxSourceGeneral.UnPause();
            musicSourceLevel.UnPause();
            sfxSourceLevel.UnPause();
            sfxSourceLevelObstacle.UnPause();
        }
    }

    /// <summary>
    /// Carga las configuraciones de volumen guardadas en PlayerPrefs y las aplica a las AudioSources.
    /// </summary>
    public void LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("GeneralMusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("GeneralSFXVolume", 1f);
        float levelSFXVolume = PlayerPrefs.GetFloat("LevelSFXVolume", 1f);

        SetGeneralMusicVolume(musicVolume);
        SetGeneralSFXVolume(sfxVolume);
        SetLevelSFXVolume(levelSFXVolume);
    }

    /// <summary>
    /// Reproduce el sonido de click utilizando la fuente SFX general.
    /// </summary>
    public void PlayClickSound()
    {
        sfxSourceGeneral.clip = AudioClipsManager.Instance.clickSound;
        sfxSourceGeneral.PlayOneShot(sfxSourceGeneral.clip);
    }

    /// <summary>
    /// Reproduce el sonido correspondiente al inicio del juego.
    /// </summary>
    public void PlayStartGameSound()
    {
        sfxSourceGeneral.clip = AudioClipsManager.Instance.StartGame;
        sfxSourceGeneral.PlayOneShot(sfxSourceGeneral.clip);
    }

    /// <summary>
    /// Reproduce el sonido de fin de juego.
    /// Nota: Se asigna al musicSourceGeneral; revisar si debería estar en otra fuente.
    /// </summary>
    public void PlayEndGameSound()
    {
        musicSourceGeneral.clip = AudioClipsManager.Instance.EndGame;
        musicSourceGeneral.PlayOneShot(musicSourceGeneral.clip);
    }

    /// <summary>
    /// Reproduce el sonido al seleccionar el siguiente botón (botón de selector).
    /// </summary>
    public void PlayClickButtonSelectorNextSound()
    {
        sfxSourceGeneral.clip = AudioClipsManager.Instance.clickButtonSelectorNext;
        sfxSourceGeneral.PlayOneShot(sfxSourceGeneral.clip);
    }

    /// <summary>
    /// Reproduce el sonido al seleccionar el botón anterior (botón de selector).
    /// </summary>
    public void PlayClickButtonSelectorBackSound()
    {
        sfxSourceGeneral.clip = AudioClipsManager.Instance.clickButtonSelectorBack;
        sfxSourceGeneral.PlayOneShot(sfxSourceGeneral.clip);
    }

    /// <summary>
    /// Reproduce el sonido al interactuar con el control de volumen.
    /// </summary>
    public void PlayClickButtonVolumeSound()
    {
        sfxSourceGeneral.clip = AudioClipsManager.Instance.clickButtonVolume;
        sfxSourceGeneral.PlayOneShot(sfxSourceGeneral.clip);
    }

    /// <summary>
    /// Reproduce el sonido del botón de volumen específico para el nivel.
    /// Se ajusta según el nombre de la escena.
    /// </summary>
    /// <param name="namescene">Nombre de la escena actual</param>
    public void PlayClickButtonVolumeSoundLevelSFX(string namescene)
    {
        // Ajusta el audio según la escena actual
        if (namescene == "LevelFE" || namescene == "TutorialFE")
        {
            sfxSourceLevel.clip = AudioClipsManager.Instance.clickButtonVolumeLevelSFX2;
        }
        else
        {
            sfxSourceLevel.clip = AudioClipsManager.Instance.clickButtonVolumeLevelSFX;
        }
        sfxSourceLevel.PlayOneShot(sfxSourceLevel.clip);
    }

    /// <summary>
    /// Reproduce el sonido de colisión con un enemigo.
    /// </summary>
    public void PlayCollisionObstacle()
    {
        sfxSourceLevel.clip = AudioClipsManager.Instance.ObstacleCarCollision;
        sfxSourceLevel.PlayOneShot(sfxSourceLevel.clip);
    }

    /// <summary>
    /// Reproduce el sonido al aparecer un enemigo.
    /// </summary>
    public void PlaySpawnObstacle()
    {
        sfxSourceLevelObstacle.clip = AudioClipsManager.Instance.ObstacleCarSpawn;
        sfxSourceLevelObstacle.PlayOneShot(sfxSourceLevelObstacle.clip);
    }

    /// <summary>
    /// Reproduce el sonido de colisión con una moneda.
    /// </summary>
    public void PlayCollisionCoin()
    {
        sfxSourceLevel.clip = AudioClipsManager.Instance.RewardCoinCollision;
        sfxSourceLevel.PlayOneShot(sfxSourceLevel.clip);
    }

    /// <summary>
    /// Reproduce el sonido de colisión con una gema.
    /// </summary>
    public void PlayCollisionGem()
    {
        sfxSourceLevel.clip = AudioClipsManager.Instance.RewardGemCollsion;
        sfxSourceLevel.PlayOneShot(sfxSourceLevel.clip);
    }

    /// <summary>
    /// Reproduce el sonido de la sección de tutoriales.
    /// </summary>
    public void PlayTutorialSound()
    {
        sfxSourceLevel.clip = AudioClipsManager.Instance.TutorialSound;
        sfxSourceLevel.PlayOneShot(sfxSourceLevel.clip);
    }

    // Métodos para el control de volúmenes

    /// <summary>
    /// Ajusta el volumen general de la música y lo guarda en PlayerPrefs.
    /// </summary>
    /// <param name="value">Nuevo valor de volumen (0 a 1)</param>
    public void SetGeneralMusicVolume(float value)
    {
        musicSourceGeneral.volume = value;
        PlayerPrefs.SetFloat("GeneralMusicVolume", value);
    }

    /// <summary>
    /// Ajusta el volumen general de los SFX y lo guarda en PlayerPrefs.
    /// </summary>
    /// <param name="value">Nuevo valor de volumen (0 a 1)</param>
    public void SetGeneralSFXVolume(float value)
    {
        sfxSourceGeneral.volume = value;
        PlayerPrefs.SetFloat("GeneralSFXVolume", value);
    }

    /// <summary>
    /// Ajusta el volumen de la música del nivel y lo guarda en PlayerPrefs.
    /// </summary>
    /// <param name="value">Nuevo valor de volumen (0 a 1)</param>
    public void SetLevelMusicVolume(float value)
    {
        musicSourceLevel.volume = value;
        PlayerPrefs.SetFloat("LevelMusicVolume", value);
    }

    /// <summary>
    /// Ajusta el volumen de los SFX del nivel, lo guarda en PlayerPrefs y actualiza el volumen de sonidos de enemigos según esté muteado.
    /// </summary>
    /// <param name="value">Nuevo valor de volumen (0 a 1)</param>
    public void SetLevelSFXVolume(float value)
    {
        sfxSourceLevel.volume = value;
        PlayerPrefs.SetFloat("LevelSFXVolume", value);

        if (!isObstacleMuted)
        {
            sfxSourceLevelObstacle.volume = value;
        }
    }

    /// <summary>
    /// Muta o desmuta los sonidos de los obstaculos.
    /// </summary>
    /// <param name="val">true para mutear, false para restaurar el volumen</param>
    public void MuteObstacleSounds(bool val)
    {
        isObstacleMuted = val;

        if (val)
        {
            sfxSourceLevelObstacle.volume = 0;
        }
        else
        {
            float volume = PlayerPrefs.GetFloat("LevelSFXVolume", 1f);
            sfxSourceLevelObstacle.volume = volume;
        }
    }
}