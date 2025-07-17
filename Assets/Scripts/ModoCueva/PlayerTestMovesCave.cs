using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Prueba de movimientos del jugador en el modo Cueva utilizando el giroscopio del Joy-Con.
/// Se detectan gestos de flexión y extensión para cambiar de carril basándose en la rotación acumulada.
/// </summary>
public class PlayerTestMovesCave : MonoBehaviour
{
    private List<Joycon> joycons;       // Lista de Joy-Con disponibles
    private Joycon joycon;              // Joy-Con seleccionado
    public int jc_ind = 0;              // Índice del Joy-Con seleccionado

    [Header("Configuración del Joy-Con")]
    private bool isLeftJoycon = true;   // Indica si se utiliza el Joy-Con izquierdo
    public float threshold = 2.46f;     // Umbral para detectar el gesto de movimiento

    [Header("Posiciones de Carril")]
    public float[] laneYPositions = { 0.1f, -0.18f, -0.48f }; // 0 = arriba, 1 = centro, 2 = abajo
    public float fixedX = 0.8f;         // Posición X fija
    private int currentLane = 1;        // Carril actual (por defecto, centrado)

    [Header("Configuración de Movimiento")]
    private bool canMove = true;        // Indica si se permite el movimiento
    private bool isMoving = false;      // Indica si el objeto se encuentra en movimiento
    private float targetY;              // Posición Y destino
    public float moveSpeed = 2.0f;      // Velocidad de movimiento
    private Animator animator;          // Referencia al Animator para reproducir animaciones

    // Acumulación del ángulo en el eje X (en grados)
    private float accumulatedAngleX = 0f;

    private float smoothedGyroX = 0f; // Valor suavizado del giroscopio X
    [Range(0.01f, 0.2f)]
    public float gyroSmoothingFactor = 0.05f; // Factor de suavizado
    public float deadZone = 0.3f; // Zona muerta para ignorar pequeños movimientos


    void Start()
    {
        // Cargar datos desde GameData para obtener el umbral configurado
        GameData gameData = GameData.Load();
        threshold = gameData.thresholdAngleFE;

        // Actualizar y seleccionar el Joy-Con adecuado
        UpdateJoyconIndex();
        // Obtener referencia al Animator
        animator = GetComponent<Animator>();

        // Posicionar al personaje en el centro
        transform.position = new Vector3(fixedX, laneYPositions[1], transform.position.z);
        targetY = laneYPositions[1];

    }

    /// <summary>
    /// Metodo para reiniciar la posición del jugador al carril central.
    /// </summary>
    public void ResetToCenter()
    {
        currentLane = 1;
        transform.position = new Vector3(fixedX, laneYPositions[currentLane], transform.position.z);
        targetY = laneYPositions[currentLane];
        canMove = true;
        isMoving = false;
        accumulatedAngleX = 0f;
        smoothedGyroX = 0f;
    }

    /// <summary>
    /// Actualiza el índice y la referencia del Joy-Con según la configuración del usuario.
    /// </summary>
    public void UpdateJoyconIndex() {
        GameData gameData = GameData.Load();
        isLeftJoycon = gameData.isLeftJoyCon;
        joycons = JoyconManager.Instance.j;

        for (int i = 0; i < joycons.Count; i++)
        {
            if (joycons[i].isLeft == isLeftJoycon)
            {
                joycon = joycons[i];
                jc_ind = i;
                break;
            }
        }
    }

    void FixedUpdate()
    {
        // Actualizar el umbral desde GameData
        GameData gameData = GameData.Load();
        threshold = gameData.thresholdAngleFE;

        // Si ya se está moviendo, actualizar posición hacia targetY
        if (isMoving)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.fixedDeltaTime);
            transform.position = new Vector3(fixedX, newY, transform.position.z);

            if (Mathf.Approximately(transform.position.y, targetY))
            {
                isMoving = false;
                // Solo permitir nuevos movimientos si está en el centro Y en zona neutra
                canMove = (currentLane == 1);
                if (currentLane == 1)
                {
                    accumulatedAngleX = 0f;
                    smoothedGyroX = 0f;
                }
                return;
            }
        }

        // No procesar si no hay Joy-Con o no se permite el movimiento
        if (joycon == null || !canMove) return;

        // Obtener la velocidad angular en X del Joy-Con
        float gyroX = joycon.GetGyro().x;
        if (isLeftJoycon) gyroX = -gyroX; // Invertir si es Joy-Con izquierdo

        // Filtro de suavizado exponencial para reducir el ruido
        smoothedGyroX = Mathf.Lerp(smoothedGyroX, gyroX, gyroSmoothingFactor);

        // Aplicar zona muerta: ignorar pequeños movimientos
        float processedGyroX = Mathf.Abs(smoothedGyroX) < deadZone ? 0f : smoothedGyroX;

        // Convertir a grados y calcular el cambio de rotación (∫ ω dt)
        float deltaRotation = processedGyroX * Mathf.Rad2Deg * Time.fixedDeltaTime;
        accumulatedAngleX += deltaRotation;

        bool moved = false;

        // Detectar gesto de "extensión" (rotación positiva) desde el centro
        if (accumulatedAngleX > threshold && currentLane == 1)
        {
            SetLane(0);
            PlayMoveUpAnimation();
            Debug.Log($"[MOVIMIENTO] Extensión | Ángulo acumulado: {accumulatedAngleX:F2}° | Threshold: {threshold}° | Carril: {currentLane}");
            moved = true;
        }
        // Detectar gesto de "flexión" (rotación negativa) desde el centro
        else if (accumulatedAngleX < -threshold && currentLane == 1)
        {
            SetLane(2);
            PlayMoveDownAnimation();
            Debug.Log($"[MOVIMIENTO] Flexión | Ángulo acumulado: {accumulatedAngleX:F2}° | Threshold: {threshold}° | Carril: {currentLane}");
            moved = true;
        }

        if (moved)
        {
            // Reiniciar acumulación de ángulo después de un movimiento
            accumulatedAngleX = 0f; 
        }

        // Resetear acumulador si vuelve a zona neutra (evita movimientos dobles)
        if (Mathf.Abs(processedGyroX) < deadZone)
        {
            accumulatedAngleX = 0f;
        }
    }

    /// <summary>
    /// Configura el destino, deshabilita temporalmente el movimiento y programa el retorno al centro si es necesario.
    /// </summary>
    /// <param name="lane">Carril de destino: 0 (arriba), 1 (centro), 2 (abajo).</param>
    void SetLane(int lane)
    {
        currentLane = lane;
        canMove = false;
        isMoving = true;

        targetY = laneYPositions[lane];
        // Si se sale del centro, iniciar retorno al centro tras un retardo
        if (lane != 1)
        {
            StartCoroutine(ReturnToCenterAfterDelay(2.45f));
        }
    }

    /// <summary>
    /// Realiza el retorno automático al centro luego de un tiempo de espera.
    /// </summary>
    /// <param name="delay">Tiempo de espera en segundos.</param>
    IEnumerator ReturnToCenterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Si se está en posición superior, reproducir animación de bajada; si está inferior, animación de subida.
        if (currentLane == 0)
        {
            PlayMoveDownAnimation();
        }
        else if (currentLane == 2) 
        {
            PlayMoveUpAnimation();
        }
        SetLane(1); // Retornar al centro
    }

    /// <summary>
    /// Reproduce la animación de movimiento hacia arriba.
    /// </summary>
    void PlayMoveUpAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("MoveUp");
        }
    }

    /// <summary>
    /// Reproduce la animación de movimiento hacia abajo.
    /// </summary>
    void PlayMoveDownAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("MoveDown");
        }
    }
}
