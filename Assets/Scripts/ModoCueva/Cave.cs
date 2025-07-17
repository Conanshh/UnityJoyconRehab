using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cave : MonoBehaviour
{
    [Header("Timer Settings")]
    public float gameTime = 60f; // Duración total del nivel en segundos.
    public float gemSpawnThreshold = 30.0f; // Tiempo en dónde se aumenta aparición de Gemas.
    public Image timeBar; // Barra de tiempo que se muestra en la interfaz.
    private float maxGameTime; // Tiempo total inicial para cálculos de progresión.
    private bool isRunning = true; // Indica si el temporizador se está actualizando.
    private GameData gameData; // Variable para acceder a los datos persistentes del juego.

    [Header("Gems Settings")]
    public TextMeshProUGUI HighScoreText; // Texto para mostrar el puntaje más alto.

    /// <summary>
    /// Realiza tareas de inicialización antes de Start.
    /// Reinicia el temporizador en el MovementLogger si este existe.
    /// </summary>
    void Awake()
    {
        if (MovementLogger.Instance != null)
        {
            MovementLogger.Instance.ResetTimer(isFE: true);
        }
    }

    /// <summary>
    /// Inicializa el nivel, configurando el tiempo de juego y mostrando el puntaje más alto en la UI.
    /// Según la escena activa, se determina si el nivel es de tutorial o normal.
    /// </summary>
    void Start()
    {
        // Configuración para escena de tutorial: asigna un tiempo fijo.
        if (SceneManager.GetActiveScene().name == "TutorialFE")
        {
            gameTime = 60f;
        }
        else
        {
            // Se cargan los datos persistentes y se asigna el tiempo de juego desde GameData.
            gameData = GameData.Load();
            gameTime = gameData.GameTime;

            // Se actualiza la interfaz con el puntaje más alto.
            int highScore = gameData.highScoreFE;
            HighScoreText.text = "Puntaje Mayor: " + highScore.ToString();
        }

        // Se guarda el tiempo total inicial para el correcto funcionamiento de la barra de tiempo.
        maxGameTime = gameTime;
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

    /// <summary>
    /// Actualiza el temporizador del nivel, ajusta la barra de tiempo y gestiona el fin de la partida.
    /// </summary>
    void Update()
    {
        if (isRunning)
        {
            // Se decrementa el tiempo del nivel utilizando el tiempo transcurrido por frame.
            gameTime -= Time.deltaTime;

            // Actualiza la barra de tiempo en función del tiempo restante.
            if (timeBar != null)
                timeBar.fillAmount = gameTime / maxGameTime;

            // Cuando el tiempo sea menor o igual a 30 segundos, se ajusta la frecuencia de generación de gemas.
            if (gameTime <= gemSpawnThreshold)
            {
                GemsSpawner spawner = FindFirstObjectByType<GemsSpawner>();
                if (spawner != null)
                {
                    spawner.SetSpawnTimer(2.0f);
                }
            }

            // Al agotarse el tiempo, se realizan las acciones para finalizar el nivel.
            if (gameTime <= 0)
            {
                gameTime = 0;
                isRunning = false;

                // Se obtiene al jugador y se guardan sus gemas en los datos persistentes.
                PlayerCave player = FindFirstObjectByType<PlayerCave>();
                if (player != null)
                {
                    gameData.totalGems = player.GemCount;
                    gameData.Save();
                }

                // Se actualiza el contador de recompensas y se guarda la sesión de movimientos.
                MovementLogger.Instance.SetRewardCount(gameData.totalGems, isFE: true);
                MovementLogger.Instance.SaveSession();

                // Se pausa la música y se guarda la escena actual para referencia futura.
                AudioManager.Instance.PauseMusic();
                PlayerPrefs.SetString("LastLevelScene", SceneManager.GetActiveScene().name);
                PlayerPrefs.Save();

                // Se carga la escena de finalización del nivel.
                SceneManager.LoadScene("FinishLevel");
            }
        }
    }
}