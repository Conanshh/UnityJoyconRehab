using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Controla la lógica del jugador en el modo Carretera, 
/// gestionando colisiones, recolección de monedas y la interacción con otros sistemas (pausa, animaciones, etc.).
/// Implementa la interfaz IPlayerCarretera para inicializar las dependencias necesarias.
/// </summary>
public class PlayerRoad : MonoBehaviour, IPlayerCarretera
{
    // Eventos propios
    public event Action OnPlayerCollision;

    // Referencias a otros sistemas y componentes del nivel
    public GameObject tilemap;
    public GameObject redOverlay;
    public ObstacleSpawner obstacleSpawner;
    public RewardSpawner rewardSpawner;
    public SpawnManager spawnManager;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI messageText;

    /// <summary>
    /// Contador de monedas recolectadas.
    /// </summary>
    public int coinCount = 0;

    /// <summary>
    /// Tiempo de reactivación de scripts tras una colisión.
    /// </summary>
    public float reactivationTime = 2f;

    /// <summary>
    /// Duración y magnitud del efecto de sacudida de cámara tras una colisión.
    /// </summary>
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.2f;

    private Road tilemapScript;
    private Animator playerAnimator;
    private Animator rewardAnimator;
    private Vector3 originalCameraPosition;

    private PlayerRoadMovement movementScript;
    private float sessionElapsedTime = 0f;
    private bool isPaused = false;

    /// <summary>
    /// Inicializa las referencias necesarias para el funcionamiento del jugador.
    /// </summary>
    public void Init(
    GameObject tilemap,
    GameObject redOverlay,
    ObstacleSpawner obstacleSpawner,
    RewardSpawner rewardSpawner,
    SpawnManager spawnManager,
    TextMeshProUGUI coinText,
    TextMeshProUGUI messageText,
    GameObject pausePanel,
    GameObject backText)
    {
        this.tilemap = tilemap;
        this.redOverlay = redOverlay;
        this.obstacleSpawner = obstacleSpawner;
        this.rewardSpawner = rewardSpawner;
        this.spawnManager = spawnManager;
        this.coinText = coinText;
        this.messageText = messageText;
    }

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        if (rewardSpawner != null)
            rewardAnimator = rewardSpawner.GetComponent<Animator>();
        if (tilemap != null)
            tilemapScript = tilemap.GetComponent<Road>();
        if (Camera.main != null)
            originalCameraPosition = Camera.main.transform.position;
        if (redOverlay != null)
            redOverlay.SetActive(false);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
        
        movementScript = GetComponent<PlayerRoadMovement>();
        movementScript.OnCollisionDetected += HandleCollision;
        movementScript.OnCoinDetected += HandleCoin;
        
        UpdateCoinUI();

        // Suscribirse a eventos de pausa global
        PauseManager.OnPause += OnPauseGame;
        PauseManager.OnResume += OnResumeGame;
    }

    void Update()
    {
        if (!isPaused)
            sessionElapsedTime += Time.deltaTime;
    }

    public float GetSessionElapsedTime()
    {
        return sessionElapsedTime;
    }

    // Este método será llamado desde PlayerRoadMovement
    public void RegistrarMovimientoAbduccionAduccion(string tipo, float angulo, float umbral)
    {
        float timestamp = GetSessionElapsedTime();
        MovementLogger.Instance.RegisterAbductionAdductionMovement(tipo, angulo, umbral, timestamp);
    }
    /// <summary>
    /// Se ejecuta cuando el objeto es destruido, desuscribiendo eventos para evitar fugas de memoria.
    /// </summary>
    void OnDestroy()
    {
        PauseManager.OnPause -= OnPauseGame;
        PauseManager.OnResume -= OnResumeGame;
    }

    /// <summary>
    /// Actualiza la interfaz del contador de monedas.
    /// </summary>
    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Monedas: " + coinCount;
    }

    /// <summary>
    /// Incrementa la cantidad de monedas, actualiza la UI y notifica el evento.
    /// </summary>
    public void AddCoin()
    {
        coinCount++;
        UpdateCoinUI();
    }

    private void HandleCoin()
    {
        AddCoin();
    }

    /// <summary>
    /// Gestiona la colisión del jugador: detiene el movimiento, pausa spawners/animations, aplica cámara shake,
    /// reinicia los ángulos acumulados y el valor suavizado del giroscopio,
    /// muestra mensajes y posteriormente reactiva los scripts.
    /// </summary>
    public void HandleCollision()
    {
        if (movementScript != null)
        {
            movementScript.ResetMovementState();
            movementScript.ResetGyro();
            movementScript.enabled = false;
        }
        if (tilemapScript != null)
        {
            tilemapScript.enabled = false;
            tilemapScript.StopTimer();
        }
        if (spawnManager != null) spawnManager.Pause();
        if (rewardSpawner != null) rewardSpawner.DestroyAllCoins();
        if (playerAnimator != null) playerAnimator.enabled = false;
        if (obstacleSpawner != null) obstacleSpawner.PauseSpawner();
        if (obstacleSpawner != null) obstacleSpawner.DestroyAllObstacles();
        if (rewardSpawner != null) rewardSpawner.StopSpawning();
        if (rewardAnimator != null) rewardAnimator.enabled = false;
        if (redOverlay != null) redOverlay.SetActive(true);
        coinCount = 0;
        UpdateCoinUI();
        StartCoroutine(CameraShake());
        StartCoroutine(CenterPlayerAfterCollision());
        if (messageText != null)
        {
            messageText.text = "Vuelve a posición neutra";
            messageText.gameObject.SetActive(true);
        }
        Invoke(nameof(ReactivateScripts), reactivationTime);

        // Notificar a otros sistemas
        OnPlayerCollision?.Invoke();
    }

    /// <summary>
    /// Espera un breve periodo y centra al jugador.
    /// </summary>
    private IEnumerator CenterPlayerAfterCollision()
    {
        yield return new WaitForSeconds(0.5f);
        movementScript.CenterLane();
    }

    /// <summary>
    /// Reactiva los scripts, objetos tras una colisión y
    /// reinicia los ángulos acumulados y el valor suavizado del giroscopio.
    /// </summary>
    void ReactivateScripts()
    {
        if (movementScript != null)
        {
            movementScript.ResetMovementState();
            movementScript.ResetGyro();
            movementScript.enabled = true;
        }
        if (tilemapScript != null)
        {
            tilemapScript.enabled = true;
            tilemapScript.ResumeTimer();
        }
        if (playerAnimator != null) playerAnimator.enabled = true;
        if (rewardAnimator != null) rewardAnimator.enabled = true;
        if (spawnManager != null) spawnManager.Resume();
        if (obstacleSpawner != null) obstacleSpawner.ResumeSpawner();
        if (rewardSpawner != null) rewardSpawner.StartSpawning();
        if (redOverlay != null) redOverlay.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Aplica un efecto de sacudida a la cámara tras la colisión.
    /// </summary>
    IEnumerator CameraShake()
    {
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * shakeIntensity;
            Camera.main.transform.position = originalCameraPosition + randomOffset;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = originalCameraPosition;
    }

    /// <summary>
    /// Aplica un efecto de sacudida a la cámara tras la colisión.
    /// </summary>
    public void HandleCollisionTutorial()
    {
        if (movementScript != null) movementScript.enabled = false;
        if (tilemapScript != null)
        {
            tilemapScript.enabled = false;
            tilemapScript.StopTimer();
        }
        if (playerAnimator != null) playerAnimator.enabled = false;
        if (redOverlay != null) redOverlay.SetActive(true);
        coinCount = 0;
        UpdateCoinUI();
        StartCoroutine(CameraShake());
        StartCoroutine(CenterPlayerAfterCollision());
        if (messageText != null)
        {
            messageText.text = "Chocaste, esquiva bien.";
            messageText.gameObject.SetActive(true);
        }

        var tutorialManager = FindFirstObjectByType<TutorialRoad>();
        if (tutorialManager != null)
            tutorialManager.HandleCollisionTutorial("obstacle");

        Invoke(nameof(ReactivateScriptsTutorial), reactivationTime);
    }

    void ReactivateScriptsTutorial()
    {
        if (movementScript != null) movementScript.enabled = true;
        if (tilemapScript != null)
        {
            tilemapScript.enabled = true;
            tilemapScript.ResumeTimer();
        }
        if (playerAnimator != null) playerAnimator.enabled = true;
        if (rewardAnimator != null) rewardAnimator.enabled = true;
        if (redOverlay != null) redOverlay.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Lógica cuando se detiene el nivel en el modo Carretera.
    /// </summary>    
    public void OnPauseGame()
    {
        movementScript.enabled = false;
        if (playerAnimator != null) playerAnimator.enabled = false;
    }

    /// <summary>
    /// Lógica para reanudar al jugador en el modo Carretera.
    /// </summary>
    public void OnResumeGame()
    {
        movementScript.enabled = true;
        if (playerAnimator != null) playerAnimator.enabled = true;
    }
}