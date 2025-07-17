using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clase que gestiona el menú de pausa para el modo Carretera, 
/// permitiendo reanudar, reiniciar o salir a las selecciones o menú principal, 
/// así como ajustar los volúmenes de música y efectos.
/// </summary>
public class MenuRoad : MonoBehaviour
{
    /// <summary>
    /// Referencia al script del jugador.
    /// </summary>
    private PlayerRoad playerScript;

    /// <summary>
    /// Referencia al gestor de pausa global.
    /// </summary>
    public PauseManager pauseManager;

    [Header("Volumen General de Música")]
    public TextMeshProUGUI musicVolumeText;
    public float musicVolumeStep = 0.05f;

    [Header("Volumen de Efectos del Nivel")]
    public TextMeshProUGUI sfxVolumeText;
    public float sfxVolumeStep = 0.05f; // Paso de aumento/disminución

    [Header("Volumen de Efectos Generales")]
    public TextMeshProUGUI sfxGeneralVolumeText;
    public float sfxGeneralVolumeStep = 0.05f; // Paso de aumento/disminución

    [Header("Control de Obstáculos")]
    public Button muteButton;              // Botón para alternar el silencio de los obstáculos.
    private bool areObstacleMuted = false; // Estado de silencio de los obstáculos.

    /// <summary>
    /// Inicializa el menú obteniendo la referencia del jugador y actualizando la UI de volúmenes.
    /// </summary>
    private void Start()
    {
        playerScript = FindFirstObjectByType<PlayerRoad>();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Inicia el nivel cargando la escena correspondiente. 
    /// Útil para el menu del tutorial.
    /// </summary>
    public void StartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelAbAd");
    }

    /// <summary>
    /// Reanuda el juego tras la pausa, ocultando el panel de pausa y reactivando el flujo del juego.
    /// </summary>
    public void ContinueGame()
    {
        if (playerScript != null)
        {
            AudioManager.Instance.PlayClickSound();
            pauseManager.ResumeGame();
            playerScript.OnResumeGame();
        }
    }

    /// <summary>
    /// Reinicia la escena actual, reiniciando el nivel.
    /// </summary>
    public void RestartGame()
    {
        AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Regresa al menú previo del nivel. Guarda la sesión en caso de tener movimientos registrados.
    /// </summary>
    public void BackGame()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayClickSound();
        if (SceneManager.GetActiveScene().name == "TutorialAbAd")
            SceneManager.LoadScene("PreLevelAbAd");
        else
        {
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
            SceneManager.LoadScene("PreLevelAbAd");
        }
    }

    /// <summary>
    /// Regresa al menú principal del juego, guardando la sesión si hay movimientos.
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayClickSound();
        if (SceneManager.GetActiveScene().name == "TutorialAbAd")
            SceneManager.LoadScene("MainMenu");
        else
        {
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
    /// Incrementa el volumen de la música general, actualiza PlayerPrefs y la UI.
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
    /// Decrementa el volumen de la música general, actualizando PlayerPrefs y la UI.
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
        //Obtener nombre de la escena
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
        currentVolume = Mathf.Clamp01(currentVolume + sfxGeneralVolumeStep);
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
        currentVolume = Mathf.Clamp01(currentVolume - sfxGeneralVolumeStep);
        AudioManager.Instance.SetGeneralSFXVolume(currentVolume);
        AudioManager.Instance.PlayClickButtonVolumeSound();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Alterna el estado de mute de los obstáculos y actualiza el texto del botón.
    /// </summary>
    public void ToggleMuteWithButton()
    {
        areObstacleMuted = !areObstacleMuted;
        AudioManager.Instance.MuteObstacleSounds(areObstacleMuted);
        muteButton.GetComponentInChildren<TextMeshProUGUI>().text = areObstacleMuted ? "Autos: Silenciados" : "Autos: Sonando";
    }

    /// <summary>
    /// Actualiza los textos de los volúmenes en la interfaz.
    /// </summary>
    private void UpdateVolumeTexts()
    {
        float musicVolume = PlayerPrefs.GetFloat("GeneralMusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("LevelSFXVolume", 1f);
        float sfxGeneralVolume = PlayerPrefs.GetFloat("GeneralSFXVolume", 1f);

        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100f) + "%";

        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(sfxVolume * 100f) + "%";
        
        if (sfxGeneralVolumeText != null)
            sfxGeneralVolumeText.text = Mathf.RoundToInt(sfxGeneralVolume * 100f) + "%";
    }

    #endregion

}
