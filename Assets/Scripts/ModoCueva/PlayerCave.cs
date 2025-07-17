using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la lógica del jugador en el modo Cueva, gestionando la recolección de gemas y los estados de
/// pausa y reanudación. Implementa la interfaz IPlayerCueva para asegurar la inicialización con las
/// dependencias necesarias.
/// </summary>
public class PlayerCave : MonoBehaviour, IPlayerCueva
{
    [Header("UI")]
    public TextMeshProUGUI GemText;  // Referencia al texto del contador de gemas
    public GemsSpawner gemsSpawner;  // Referencia al spawner de gemas
    public GameObject pausePanel;    // Referencia al panel de pausa
    public GameObject backText;      // Referencia al texto de fondo
    public GameObject redOverlay;    // Imagen roja transparente

    [Header("Contadores y Estados")]
    public int GemCount = 0;

    // Otras referencias
    private Animator animator;                // Referencia al Animator para las animaciones
    public GameObject feedbackText;           // Para mostrar mensajes de feedback
    public CameraShake cameraShake;           // Componente para efecto de sacudida de cámara

    public PlayerCaveMovement movementScript; // Referencia al script de movimiento del jugador
    private float sessionElapsedTime = 0f; // Tiempo transcurrido en la sesión actual
    private bool isPaused = false; // Indica si el juego está en pausa

    #region Inicialización y Configuración
    /// <summary>
    /// Inicializa al jugador con las referencias necesarias para el modo Cueva.
    /// </summary>
    public void Init(TextMeshProUGUI GemText, GemsSpawner gemsSpawner, GameObject pausepanel,
                     GameObject backText, GameObject feedbackText, CameraShake camera, GameObject redOverlay)
    {
        this.GemText = GemText;
        this.gemsSpawner = gemsSpawner;
        this.pausePanel = pausepanel;
        this.backText = backText;
        this.feedbackText = feedbackText;
        this.cameraShake = camera;
        this.redOverlay = redOverlay;
    }

    void Start()
    {
        // Inicializar el contador de gemas
        UpdateGemUI();

        // Suscribirse a los eventos globales de pausa/reanudación emitidos por PauseManager
        PauseManager.OnPause += OnGamePaused;
        PauseManager.OnResume += OnGameResumed;


        // Obtener componente de movimiento y suscribir evento de recolección de gemas
        movementScript = GetComponent<PlayerCaveMovement>();
        if (movementScript != null)
        {
            movementScript.OnGemDetected += HandleGem;
        }
    }

    void OnDestroy()
    {
        PauseManager.OnPause -= OnGamePaused;
        PauseManager.OnResume -= OnGameResumed;
        if (movementScript != null)
        {
            movementScript.OnGemDetected -= HandleGem;
        }
    }

    #endregion

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
    public void RegistrarMovimientoFlexionExtension(string tipo, float angulo, float umbral)
    {
        float timestamp = GetSessionElapsedTime();
        MovementLogger.Instance.RegisterFlexionExtensionMovement(tipo, angulo, umbral, timestamp);
    }

    #region Manejo de Gemas

    /// <summary>
    /// Maneja el evento cuando se detecta una gema.
    /// </summary>
    private void HandleGem()
    {
        AddGem();
    }

    /// <summary>
    /// Incrementa el contador de gemas y actualiza la interfaz.
    /// </summary>
    public void AddGem()
    {
        GemCount++;
        UpdateGemUI();
    }

    /// <summary>
    /// Actualiza el contador de gemas en la UI.
    /// </summary>
    private void UpdateGemUI()
    {
        if (GemText != null)
            GemText.text = "Gemas: " + GemCount;
    }

    #endregion

    #region Manejo de Pausa

    /// <summary>
    /// Aplica la pausa al jugador deteniendo el movimiento.
    /// Método requerido por la interfaz IPlayerCueva.
    /// </summary>
    public void OnGamePaused()
    {
        if (movementScript != null)
            movementScript.enabled = false;
    }

    /// <summary>
    /// Reanuda el movimiento del jugador tras una pausa.
    /// Método requerido por la interfaz IPlayerCueva.
    /// </summary>
    public void OnGameResumed()
    {
        if (movementScript != null)
            movementScript.enabled = true;
    }

    #endregion

}
