using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Configura y reproduce la música asociada a la escena actual.
/// Carga los ajustes de volumen y asigna el clip adecuado según el valor del enum SceneMusic.
/// </summary>
public class MusicStart : MonoBehaviour
{
    /// <summary>
    /// Enum para seleccionar la escena asociada y, por lo tanto, el clip de música a reproducir.
    /// </summary>
    public SceneMusic sceneMusic;

    /// <summary>
    /// Inicializa la música: carga volúmenes, asigna el clip adecuado y lo reproduce.
    /// </summary>
    void Start()
    {
        SetupMusic();
    }

    /// <summary>
    /// Configura la música según la escena seleccionada:
    /// - Carga los ajustes de volumen.
    /// - Asigna el clip de música adecuado.
    /// - Reproduce la música.
    /// </summary>
    private void SetupMusic()
    {
        // Cargar los ajustes de volumen previamente configurados.
        AudioManager.Instance.LoadVolumeSettings();

        // Seleccionar el clip de música dependiendo de la escena asignada.
        switch (sceneMusic)
        {
            case SceneMusic.MainMenu:
                AudioManager.Instance.musicSourceGeneral.clip = AudioClipsManager.Instance.mainMenuMusic;
                break;
            case SceneMusic.PreLevelAbAd:
                AudioManager.Instance.musicSourceGeneral.clip = AudioClipsManager.Instance.prelevelAbAdMusic;
                break;
            case SceneMusic.PreLevelFE:
                AudioManager.Instance.musicSourceGeneral.clip = AudioClipsManager.Instance.prelevelFEMusic;
                break;
            case SceneMusic.LevelAbAd:
                AudioManager.Instance.musicSourceGeneral.clip = AudioClipsManager.Instance.levelAbAdMusic;
                break;
            case SceneMusic.LevelFE:
                AudioManager.Instance.musicSourceGeneral.clip = AudioClipsManager.Instance.levelFEMusic;
                break;
            case SceneMusic.Tutorial:
                AudioManager.Instance.musicSourceGeneral.clip = AudioClipsManager.Instance.tutorialMusic;
                break;
            default:
                Debug.LogWarning("SceneMusic no asignado, no se ha seleccionado un clip de música.");
                break;
        }

        // Reproducir el clip asignado.
        AudioManager.Instance.musicSourceGeneral.Play();
    } 
}

/// <summary>
/// Enum utilizado para seleccionar el clip de música según la escena.
/// </summary>
public enum SceneMusic
{
    MainMenu,
    PreLevelAbAd,
    PreLevelFE,
    LevelAbAd,
    LevelFE,
    Tutorial
}
