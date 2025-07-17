using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el movimiento del jugador en el modo Cueva mediante la detección de input del Joy-Con.
/// Controla el cambio de carril, la animación, la detección de inactividad y el retorno automático
/// al carril central.
/// </summary>
public class PlayerCaveMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 2.0f;
    public float[] laneYPositions = { 0.19f, -0.20f, -0.52f }; // 0 = arriba, 1 = centro, 2 = abajo
    public float fixedX = 0.296f;
    private int currentLane = 1;

    private bool canMove = true;
    private bool isMoving = false;
    private float targetY;

    private PlayerCave playerCave;

    private float smoothedGyroX = 0f; // Valor suavizado del giroscopio X
    [Range(0.01f, 0.2f)]
    public float gyroSmoothingFactor = 0.05f; // Factor de suavizado
    public float deadZone = 0.3f; // Zona muerta para ignorar pequeños movimientos

    [Header("Configuración de Entrada y Sensibilidad")]
    public float threshold = 20.0f; // Se actualizará desde GameData
    private float accumulatedAngleX = 0f;

    [Header("Inactividad")]
    public float idleThreshold = 6.0f;
    private float idleTimer = 0f;
    private bool wasIdle = false;

    [Header("Referencias y Animaciones")]
    public CameraShake cameraShake;
    public GameObject feedbackText;  // Para el mensaje "Vuelve al reposo"
    private Animator animator;       // Para las animaciones
    public GameObject redOverlay;    // Imagen roja transparente

    // Variables relacionadas con el Joy-Con
    private List<Joycon> joycons;
    private Joycon joycon;
    public int jc_ind = 0;
    private bool isLeftJoycon = true;

    // Eventos para notificar a PlayerCave
    public event System.Action OnGemDetected;
    public event System.Action OnRequestPause;

    /// <summary>
    /// Inicializa los componentes para efectos visuales y de feedback.
    /// </summary>
    /// <param name="cs">Componente para sacudida de cámara.</param>
    /// <param name="overlay">Imagen roja transparente.</param>
    /// <param name="feedbacktext">Objeto de texto para feedback.</param>
    public void InitializeComponents(CameraShake cs, GameObject overlay, GameObject feedbacktext)
    {
        cameraShake = cs;
        redOverlay = overlay;
        feedbackText = feedbacktext;
    }


    void Start()
    {
        playerCave = GetComponent<PlayerCave>();

        // Cargar datos de configuración
        GameData gameData = GameData.Load();
        isLeftJoycon = gameData.isLeftJoyCon;
        threshold = gameData.thresholdAngleFE;

        // Seleccionar el Joy-Con elegido (izquierdo o derecho)
        joycons = JoyconManager.Instance.j;
        MovementLogger.Instance.SetSessionInfo(isFE: true, thresholdAngle: threshold, axis: "Gyro X", gameTime: gameData.GameTime, isLeftJoycon);
        
        for (int i = 0; i < joycons.Count; i++)
        {
            if (joycons[i].isLeft == isLeftJoycon)
            {
                joycon = joycons[i];
                jc_ind = i;
                break;
            }
        }

        // Establecer posición inicial en el carril central
        currentLane = 1;
        targetY = laneYPositions[currentLane];
        transform.position = new Vector3(fixedX, targetY, transform.position.z);

        // Obtener el Animator y desactivar feedback si existe
        animator = GetComponent<Animator>();
        if (feedbackText != null) feedbackText.SetActive(false);
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
            return;
        }

        if (joycon == null || !canMove) return;

        ProcessJoyConInput();
    }

    /// <summary>
    /// Mueve el objeto hacia la posición objetivo en el eje Y.
    /// </summary>
    private void MoveTowardsTarget()
    {
        float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(fixedX, newY, transform.position.z);
        if (Mathf.Approximately(transform.position.y, targetY))
        {
            isMoving = false;
            // Permitir movimiento sólo si se está en el carril central
            canMove = (currentLane == 1);
            if (currentLane == 1)
            {
                accumulatedAngleX = 0f;
                smoothedGyroX = 0f;
            }
        }
    }

    /// <summary>
    /// Procesa la entrada del Joy-Con para detectar movimientos de extensión o flexión y gestiona la inactividad.
    /// </summary>
    private void ProcessJoyConInput()
    {
        // Leer velocidad angular en X (invertir si es Joy-Con izquierdo)
        float gyroX = joycon.GetGyro().x;
        if (isLeftJoycon) gyroX = -gyroX;

        // Filtro de suavizado exponencial para reducir el ruido
        smoothedGyroX = Mathf.Lerp(smoothedGyroX, gyroX, gyroSmoothingFactor);

        // Aplicar zona muerta: ignorar pequeños movimientos
        float processedGyroX = Mathf.Abs(smoothedGyroX) < deadZone ? 0f : smoothedGyroX;

        // Convertir a grados y calcular el cambio de rotación (∫ ω dt)
        float deltaRotation = processedGyroX * Mathf.Rad2Deg * Time.fixedDeltaTime;
        accumulatedAngleX += deltaRotation;

        bool moved = false;
        if (accumulatedAngleX > threshold && currentLane == 1)
        {
            //MovementLogger.Instance.RegisterFlexionExtensionMovement("Extensión", accumulatedAngleX, threshold);
            if (!SceneManager.GetActiveScene().name.ToLower().Contains("tutorial") && playerCave != null)
            {
                // Solo registrar si NO es la escena de tutorial
                playerCave.RegistrarMovimientoFlexionExtension("Extensión", accumulatedAngleX, threshold);
            }
            Debug.Log($"[MOVIMIENTO] Extensión | Ángulo acumulado: {accumulatedAngleX:F2}° | Threshold: {threshold}° | Carril: {currentLane}");
            SetLane(0);
            PlayMoveUpAnimation();
            moved = true;
        }
        else if (accumulatedAngleX < -threshold && currentLane == 1)
        {
            //MovementLogger.Instance.RegisterFlexionExtensionMovement("Flexión", accumulatedAngleX, threshold);
            if (!SceneManager.GetActiveScene().name.ToLower().Contains("tutorial") && playerCave != null)
            {
                // Solo registrar si NO es la escena de tutorial
                playerCave.RegistrarMovimientoFlexionExtension("Flexión", accumulatedAngleX, threshold);
            }
            Debug.Log($"[MOVIMIENTO] Flexión | Ángulo acumulado: {accumulatedAngleX:F2}° | Threshold: {threshold}° | Carril: {currentLane}");
            SetLane(2);
            PlayMoveDownAnimation();
            moved = true;
        }

        if (moved)
        {
            accumulatedAngleX = 0f;
            idleTimer = 0f;
            wasIdle = false;
        }

        // Resetear acumulador si vuelve a zona neutra (evita movimientos dobles)
        if (Mathf.Abs(processedGyroX) < deadZone)
        {
            accumulatedAngleX = 0f;
            idleTimer += Time.fixedDeltaTime;
            if (idleTimer >= idleThreshold && !wasIdle)
            {
                if (cameraShake != null)
                {
                    StartCoroutine(ShakeAndPause());
                }
            }
        }
    }

    /// <summary>
    /// Cambia el carril actual y activa la animación correspondiente.
    /// Si se cambia a un carril distinto del central, se programa un retorno al centro.
    /// </summary>
    /// <param name="lane">Índice del carril destino: 0 (arriba), 1 (centro), 2 (abajo).</param>
    public void SetLane(int lane)
    {
        currentLane = lane;
        canMove = false;
        isMoving = true;
        targetY = laneYPositions[lane];
        if (lane != 1)
        {
            StartCoroutine(ReturnToCenterAfterDelay(2.5f));
        }
    }

    /// <summary>
    /// Espera el tiempo especificado, muestra un mensaje y retorna automáticamente al carril central.
    /// </summary>
    /// <param name="delay">Tiempo a esperar antes de retornar al centro.</param>
    IEnumerator ReturnToCenterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowReturnToCenterMessage();
        if (currentLane == 0)
        {
            PlayMoveDownAnimation();
        }
        else if (currentLane == 2)
        {
            PlayMoveUpAnimation();
        }
        SetLane(1);
        wasIdle = false;
    }

    /// <summary>
    /// Muestra el mensaje de retorno al carril central y lo borra tras un breve período.
    /// </summary>
    void ShowReturnToCenterMessage()
    {
        if (feedbackText != null)
        {
            feedbackText.SetActive(true);
            StartCoroutine(ClearMessageAfterDelay(1.3f));
        }
    }

    /// <summary>
    /// Remueve el mensaje de feedback después del retardo especificado.
    /// </summary>
    /// <param name="delay">Tiempo de espera antes de ocultar el mensaje.</param>
    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText != null)
            feedbackText.SetActive(false);
    }

    /// <summary>
    /// Ejecuta un efecto de sacudida en la cámara y luego invoca el evento para solicitar una pausa.
    /// Además, resetea las variables de inactividad y acumulación de ángulo.
    /// </summary>
    IEnumerator ShakeAndPause()
    {
        // Reiniciar variables relevantes
        accumulatedAngleX = 0f;
        smoothedGyroX = 0f;
        idleTimer = 0f;
        wasIdle = false;

        if (redOverlay != null) redOverlay.SetActive(true);
        cameraShake.Shake(1.5f, 0.4f); // Duración e intensidad del temblor
        yield return new WaitForSeconds(1.5f);
        if (redOverlay != null) redOverlay.SetActive(false);
        OnRequestPause?.Invoke();
    }

    #region Animaciones
    /// <summary>
    /// Activa la animación de movimiento hacia arriba.
    /// </summary>
    void PlayMoveUpAnimation()
    {
        if (animator != null)
            animator.SetTrigger("MoveUp");
    }

    /// <summary>
    /// Activa la animación de movimiento hacia abajo.
    /// </summary>
    void PlayMoveDownAnimation()
    {
        if (animator != null)
            animator.SetTrigger("MoveDown");
    }
    #endregion

    #region Detección de Colisiones
    /// <summary>
    /// Detecta colisiones con objetos etiquetados como "Gem" y notifica el evento correspondiente.
    /// </summary>
    /// <param name="other">Collider del objeto con el que se colisiona.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gem"))
        {
            OnGemDetected?.Invoke();
            Destroy(other.gameObject);
        }
    }
    #endregion
}