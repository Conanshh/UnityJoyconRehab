using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Finaliza la aplicación.
/// </summary>
public class MenuCave : MonoBehaviour
{
    private PlayerCave playerScript;

    [Tooltip("Referencia al PauseManager que maneja la pausa en el juego.")]
    public PauseManager pauseManager;

    [Header("Volumen General de Música")]
    public TextMeshProUGUI musicVolumeText;
    public float musicVolumeStep = 0.05f;

    [Header("Volumen de Efectos")]
    public TextMeshProUGUI sfxVolumeText;
    public float sfxVolumeStep = 0.05f;

    public TextMeshProUGUI sfxVolumeTextGeneral;
    public float sfxVolumeStepGeneral = 0.05f;

    /// <summary>
    /// Inicializa el menú buscando el PlayerCave y actualizando la UI de volúmenes.
    /// </summary>
    void Start()
    {
        playerScript = FindFirstObjectByType<PlayerCave>();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Reanuda el juego: reproduce click, reanuda mediante el PauseManager y reinicia la lógica interna del jugador.
    /// </summary>
    public void ContinueGame()
    {
        if (playerScript != null)
        {
            AudioManager.Instance.PlayClickSound();
            pauseManager.ResumeGame();
            playerScript.OnGameResumed();
        }
    }

    /// <summary>
    /// Inicia el nivel reiniciando Time.timeScale y cargando la escena "LevelFE".
    /// </summary>
    public void StartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelFE"); 
    }

    /// <summary>
    /// Reinicia el juego actual, reanudando la escena actual.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Regresa al menú de selección de niveles. Si se trata del tutorial o del nivel normal, guarda la sesión 
    /// en caso de haber movimientos registrados.
    /// </summary>
    public void BackGame()
    {
        // Si es el tutorial, carga PreLevelFE directamente.
        if (SceneManager.GetActiveScene().name == "TutorialFE")
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("PreLevelFE");
        }
        else
        {
            Time.timeScale = 1; 
            // Verifica si hay al menos un movimiento registrado
            var logger = MovementLogger.Instance;
            bool hayMovimientos =
                logger.lastWasFE
                    ? logger.FullSessionData.flexionExtension.movements.Count > 0
                    : logger.FullSessionData.abduccionAduccion.movements.Count > 0;

            if (hayMovimientos)
            {
                logger.SaveSession();
            }
            SceneManager.LoadScene("PreLevelFE");
        }
    }

    /// <summary>
    /// Retorna al menú principal. Si es tutorial, carga MainMenu directamente, y en otros casos guarda la sesión.
    /// </summary>
    public void GoToMainMenu()
    {
        if (SceneManager.GetActiveScene().name == "TutorialFE")
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu"); 
        }
        else
        {
            Time.timeScale = 1; 
            // Verifica si hay al menos un movimiento registrado
            var logger = MovementLogger.Instance;
            bool hayMovimientos =
                logger.lastWasFE
                    ? logger.FullSessionData.flexionExtension.movements.Count > 0
                    : logger.FullSessionData.abduccionAduccion.movements.Count > 0;

            if (hayMovimientos)
            {
                logger.SaveSession();
            }
            SceneManager.LoadScene("MainMenu");
        }
    }

    #region Métodos de Ajuste de Volumen

    /// <summary>
    /// Incrementa el volumen de la música general y actualiza la UI.
    /// </summary>    
    public void IncreaseMusicVolume()
    {
        float currentVolume = PlayerPrefs.GetFloat("GeneralMusicVolume", 1f);
        currentVolume = Mathf.Clamp01(currentVolume + musicVolumeStep);
        AudioManager.Instance.SetGeneralMusicVolume(currentVolume);
        AudioManager.Instance.PlayClickButtonVolumeSound();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Decrementa el volumen de la música general y actualiza la UI.
    /// </summary>
    public void DecreaseMusicVolume()
    {
        float currentVolume = PlayerPrefs.GetFloat("GeneralMusicVolume", 1f);
        currentVolume = Mathf.Clamp01(currentVolume - musicVolumeStep);
        AudioManager.Instance.SetGeneralMusicVolume(currentVolume);
        AudioManager.Instance.PlayClickButtonVolumeSound();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Incrementa el volumen de los efectos del nivel y actualiza la UI.
    /// </summary>
    public void IncreaseSFXVolume()
    {
        float currentVolume = PlayerPrefs.GetFloat("LevelSFXVolume", 1f);
        currentVolume = Mathf.Clamp01(currentVolume + sfxVolumeStep);
        AudioManager.Instance.SetLevelSFXVolume(currentVolume);
        string sceneName = SceneManager.GetActiveScene().name;
        AudioManager.Instance.PlayClickButtonVolumeSoundLevelSFX(sceneName);
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Decrementa el volumen de los efectos del nivel y actualiza la UI.
    /// </summary>
    public void DecreaseSFXVolume()
    {
        float currentVolume = PlayerPrefs.GetFloat("LevelSFXVolume", 1f);
        currentVolume = Mathf.Clamp01(currentVolume - sfxVolumeStep);
        AudioManager.Instance.SetLevelSFXVolume(currentVolume);
        string sceneName = SceneManager.GetActiveScene().name;
        AudioManager.Instance.PlayClickButtonVolumeSoundLevelSFX(sceneName);
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Incrementa el volumen de los efectos generales y actualiza la UI.
    /// </summary>
    public void IncreaseSFXVolumeGeneral()
    {
        float currentVolume = PlayerPrefs.GetFloat("GeneralSFXVolume", 1f);
        currentVolume = Mathf.Clamp01(currentVolume + sfxVolumeStepGeneral);
        AudioManager.Instance.SetGeneralSFXVolume(currentVolume);
        AudioManager.Instance.PlayClickButtonVolumeSound();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Decrementa el volumen de los efectos generales y actualiza la UI.
    /// </summary>
    public void DecreaseSFXVolumeGeneral()
    {
        float currentVolume = PlayerPrefs.GetFloat("GeneralSFXVolume", 1f);
        currentVolume = Mathf.Clamp01(currentVolume - sfxVolumeStepGeneral);
        AudioManager.Instance.SetGeneralSFXVolume(currentVolume);
        AudioManager.Instance.PlayClickButtonVolumeSound();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Actualiza los textos de los volúmenes según los valores almacenados en PlayerPrefs.
    /// </summary>
    private void UpdateVolumeTexts()
    {
        float musicVolume = PlayerPrefs.GetFloat("GeneralMusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("LevelSFXVolume", 1f);
        float sfxVolumeGeneral = PlayerPrefs.GetFloat("GeneralSFXVolume", 1f);

        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100f) + "%";

        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(sfxVolume * 100f) + "%";

        if (sfxVolumeTextGeneral != null)
            sfxVolumeTextGeneral.text = Mathf.RoundToInt(sfxVolumeGeneral * 100f) + "%";
    }

    #endregion

}
