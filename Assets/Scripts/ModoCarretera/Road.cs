using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla el comportamiento del fondo infinito y la lógica temporizada del nivel de Carretera.
/// Se encarga de mover el tilemap, actualizar la barra de tiempo y gestionar el final del nivel.
/// </summary>
public class Road : MonoBehaviour
{
    // Configuración de desplazamiento y posiciones
    public float speed = 0.6f;
    private const float RESET_POSITION_Y = -0.428f;
    private const float START_POSITION_Y = -0.253f;

    [Header("Timer Settings")]
    public float gameTime = 60f; // Duración total del juego en segundos
    private float maxGameTime;
    private bool isRunning = true;
    private GameData gameData; // Instancia para acceder a los datos del juego
    public float timelvl;
    
    [Header("UI")]
    public Image timeBar; // Barra de tiempo en UI
    public TextMeshProUGUI HighScoreText; 

    void Awake()
    {
        // Reinicia el temporizador registrado en MovementLogger
        if (MovementLogger.Instance != null)
        {
            MovementLogger.Instance.ResetTimer(isFE: false);
        }
    }   

    void Start()
    {
        // Configuración según la escena actual
        if (SceneManager.GetActiveScene().name == "TutorialAbAd")
        {
            // Si es la escena del tutorial, el tiempo es fijo
            gameTime = 60f; 
        }
        else
        {
            // Cargar los datos guardados desde GameData para niveles normales
            gameData = GameData.Load();
            gameTime = gameData.GameTime;

            //Obtener HighScore:
            int highScore = gameData.highScoreAbAd;
            HighScoreText.text = "Puntaje Mayor: " + highScore.ToString();
        }

        // Se guarda el tiempo total inicial
        maxGameTime = gameTime; 
    }
    void Update()
    {
        if (!isRunning)
            return;

        UpdateTimer();
        UpdateTilemapMovement();
    }

    /// <summary>
    /// Actualiza el temporizador del nivel, actualiza la barra de tiempo y verifica si se debe finalizar el nivel.
    /// </summary>
    private void UpdateTimer()
    {
        gameTime -= Time.deltaTime;
        if (timeBar != null)
            timeBar.fillAmount = gameTime / maxGameTime;

        if (gameTime <= 0)
        {
            gameTime = 0;
            isRunning = false;
            EndLevel();
        }
    }

    /// <summary>
    /// Desplaza el tilemap de forma infinita y lo reinicia al alcanzar la posición límite.
    /// </summary>
    private void UpdateTilemapMovement()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
        if (transform.position.y <= RESET_POSITION_Y)
        {
            transform.position = new Vector3(transform.position.x, START_POSITION_Y, transform.position.z);
        }
    }

    /// <summary>
    /// Maneja la finalización del nivel:
    /// - Guarda datos del jugador.
    /// - Notifica a MovementLogger.
    /// - Pausa la música.
    /// - Almacena la escena actual y carga la escena de finalización.
    /// </summary>
    private void EndLevel()
    {
        PlayerRoad player = FindFirstObjectByType<PlayerRoad>();
        if (player != null)
        {
            gameData.totalCoins = player.coinCount;
            gameData.Save();
        }

        MovementLogger.Instance?.SetRewardCount(gameData.totalCoins, isFE: false);
        MovementLogger.Instance?.SaveSession();

        AudioManager.Instance.PauseMusic();

        PlayerPrefs.SetString("LastLevelScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("FinishLevel");
    }

    /// <summary>
    /// Detiene la actualización del temporizador.
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// Reanuda la actualización del temporizador.
    /// </summary>
    public void ResumeTimer()
    {
        isRunning = true;
    }
}
