using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Clase responsable de gestionar la finalización de un nivel, mostrando la recompensa y permitiendo la navegación entre escenas.
/// </summary>
public class FinishLevel : MonoBehaviour
{
    public Button RestartButton;
    public Button BackButton;
    public Button MainMenuButton;
    public Button CopyPathButton;
    public TextMeshProUGUI JsonPathText;
    private string jsonPath;
    public TextMeshProUGUI RewardText;
    private GameData _gameData;

    // Constantes para claves de PlayerPrefs y nombres de escenas
    private const string LastLevelSceneKey = "LastLevelScene";
    private const string LevelFE = "LevelFE";
    private const string PreLevelFE = "PreLevelFE";
    private const string PreLevelAbAd = "PreLevelAbAd";
    private const string MainMenu = "MainMenu";

    /// <summary>
    /// Se ejecuta al cargar el script. Obtiene la escena previa, carga los datos del juego y actualiza el texto de recompensa según la escena.
    /// </summary>
    private void Awake()
    {
        // Obtener la última escena guardada
        string lastLevel = PlayerPrefs.GetString(LastLevelSceneKey, "");

        // Cargar los datos del juego
        _gameData = GameData.Load();
        AudioManager.Instance.PlayEndGameSound();

        if (lastLevel == LevelFE)
        {
            int totalGems = _gameData.totalGems;
            RewardText.text = $"Felicidades, completaste el nivel!\nGemas recolectadas: {totalGems}";
        }
        else
        {
            int totalCoins = _gameData.totalCoins;
            RewardText.text = $"Felicidades, completaste el nivel!\nMonedas recolectadas: {totalCoins}";
        }

        // Mostrar la ruta del archivo JSON
        jsonPath = Application.persistentDataPath;
        if (JsonPathText != null)
        {
            JsonPathText.text = $"{jsonPath}";
        }
    }

    /// <summary>
    /// Se ejecuta al iniciar la escena. Asigna los eventos a los botones correspondientes.
    /// </summary>
    private void Start()
    {
        // Asignar eventos a los botones
        RestartButton.onClick.AddListener(RestartGame);
        BackButton.onClick.AddListener(BackGame);
        MainMenuButton.onClick.AddListener(GoToMainMenu);
        CopyPathButton.onClick.AddListener(CopyJsonPathToClipboard);
    }

    private void CopyJsonPathToClipboard()
    {
        GUIUtility.systemCopyBuffer = jsonPath;
    }

    /// <summary>
    /// Reinicia el juego cargando la última escena almacenada.
    /// Reproduce un sonido de click y restablece la escala del tiempo.
    /// </summary>
    public void RestartGame()
    {
        AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1;

        string lastLevel = PlayerPrefs.GetString(LastLevelSceneKey, "");
        if (!string.IsNullOrEmpty(lastLevel))
        {
            SceneManager.LoadScene(lastLevel);
        }
        else
        {
            Debug.LogWarning("La escena previa no fue encontrada. Asegúrese de guardar la escena antes de cargar FinishLevel.");
        }
    }

    /// <summary>
    /// Regresa al menú previo dependiendo de la última escena jugada.
    /// Reproduce un sonido de click y restablece la escala del tiempo.
    /// </summary>
    public void BackGame()
    {
        AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1;

        string lastLevel = PlayerPrefs.GetString(LastLevelSceneKey, "");
        if (lastLevel == LevelFE)
        {
            SceneManager.LoadScene(PreLevelFE);
        }
        else
        {
            SceneManager.LoadScene(PreLevelAbAd);
        }
    }

    /// <summary>
    /// Regresa al menú principal del juego.
    /// Reproduce un sonido de click, restablece la escala del tiempo y carga la escena del menú.
    /// </summary>
    public void GoToMainMenu()
    {
        AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1;
        SceneManager.LoadScene(MainMenu);
    }
}