using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla el movimiento del jugador en el modo carretera usando el giroscopio del Joy-Con.
/// Aplica filtro de suavizado, zona muerta y cooldown para evitar movimientos involuntarios.
/// </summary>
public class PlayerTestMovesRoad : MonoBehaviour
{
    // Lista de Joy-Cons conectados.
    private List<Joycon> joycons;

    // Índice del Joy-Con seleccionado.
    public int jc_ind = 0;

    // Valor inicial del giroscopio en Z al iniciar.
    private float initialGyroZ;

    // Valor suavizado del giroscopio en Z.
    private float smoothedGyroZ = 0f;

    // Factor de suavizado para el filtro exponencial (más bajo = más suave).
    [Range(0.01f, 0.2f)]
    public float gyroSmoothingFactor = 0.05f;

    // Carril actual del jugador (0 = Izquierda, 1 = Centro, 2 = Derecha).
    private int currentLane = 1;

    // Posiciones fijas en X para cada carril.
    public float[] lanePositions = { -0.254f, 0.26f, 0.78f };

    // Indica si se está usando el Joy-Con izquierdo.
    public bool isLeftJoycon;

    // Instancia para acceder a los datos del juego.
    private GameData gameData;

    // Umbral de ángulo necesario para cambio de carril (en grados).
    public float rotationThresholdDeg;

    // Acumulador del ángulo rotado desde el último cambio de carril.
    private float accumulatedRotation = 0f;

    // Indica si actualmente se está detectando un movimiento de rotación.
    private bool isRotating = false;

    // Zona muerta para ignorar pequeños movimientos del giroscopio.
    public float deadZone = 0.3f;

    // Tiempo mínimo (en segundos) entre cambios de carril.
    public float laneChangeCooldown = 0.3f;

    // Marca de tiempo del último cambio de carril.
    private float lastLaneChangeTime = -1f;

    /// <summary>
    /// Inicializa las variables y configura el Joy-Con seleccionado.
    /// </summary>
    void Start()
    {
        gameData = GameData.Load();
        rotationThresholdDeg = gameData.thresholdAngleAbAd;
        isLeftJoycon = gameData.isLeftJoyCon;
        UpdateJoyconIndex();

        initialGyroZ = joycons[jc_ind].GetGyro().z;
        smoothedGyroZ = initialGyroZ;
        currentLane = 1;
        transform.position = new Vector3(lanePositions[currentLane], transform.position.y, transform.position.z);
    }

    /// <summary>
    /// Actualiza el índice del Joy-Con seleccionado según la configuración guardada.
    /// </summary>
    public void UpdateJoyconIndex()
    {
        gameData = GameData.Load();
        isLeftJoycon = gameData.isLeftJoyCon;
        joycons = JoyconManager.Instance.j;
        for (int i = 0; i < joycons.Count; i++)
        {
            if (joycons[i].isLeft == isLeftJoycon)
            {
                jc_ind = i;
                break;
            }
        }
    }

    /// <summary>
    /// Metodo para resetear la posición del jugador al carril central.
    /// </summary>
    public void ResetToCenter()
    {
        currentLane = 1;
        transform.position = new Vector3(lanePositions[currentLane], transform.position.y, transform.position.z);
        isRotating = false;
        accumulatedRotation = 0f;
    }

    /// <summary>
    /// Lógica de movimiento del jugador basada en el giroscopio, aplicando filtro, zona muerta y cooldown.
    /// </summary>
    void FixedUpdate()
    {
        if (joycons == null || joycons.Count == 0) return;

        gameData = GameData.Load();
        rotationThresholdDeg = gameData.thresholdAngleAbAd;

        Joycon j = joycons[jc_ind];
        float rawGyroZ = j.GetGyro().z;

        // Filtro de suavizado exponencial para reducir el ruido del giroscopio
        smoothedGyroZ = Mathf.Lerp(smoothedGyroZ, rawGyroZ, gyroSmoothingFactor);

        // Aplicar zona muerta: ignorar pequeños movimientos
        float processedGyroZ = Mathf.Abs(smoothedGyroZ) < deadZone ? 0f : smoothedGyroZ;

        // Solo acumular si estamos fuera de cooldown
        if (Time.time - lastLaneChangeTime > laneChangeCooldown)
        {
            // Activar detección solo si el movimiento es suficientemente grande
            if (!isRotating && Mathf.Abs(processedGyroZ) > deadZone)
            {
                isRotating = true;
            }

            if (isRotating)
            {
                // Calcular el delta de rotación en grados usando el valor suavizado
                float deltaRotation = processedGyroZ * Mathf.Rad2Deg * Time.fixedDeltaTime;
                accumulatedRotation += deltaRotation;

                if (Mathf.Abs(accumulatedRotation) >= rotationThresholdDeg)
                {
                    string moveType = "";
                    if (accumulatedRotation > 0 && currentLane < 2)
                    {
                        currentLane++;
                        moveType = isLeftJoycon ? "Aducción" : "Abducción";
                    }
                    else if (accumulatedRotation < 0 && currentLane > 0)
                    {
                        currentLane--;
                        moveType = isLeftJoycon ? "Abducción" : "Aducción";
                    }

                    if (!string.IsNullOrEmpty(moveType))
                    {
                        Debug.Log($"[MOVIMIENTO] {moveType} | Ángulo acumulado: {accumulatedRotation:F2}° | Threshold: {rotationThresholdDeg}° | Carril: {currentLane}");
                        transform.position = new Vector3(lanePositions[currentLane], transform.position.y, transform.position.z);
                        lastLaneChangeTime = Time.time; // Activar cooldown
                    }

                    accumulatedRotation = 0f;
                    isRotating = false;
                }
                // Esperar a volver a zona neutra antes de permitir otro movimiento
                else if (Mathf.Abs(processedGyroZ) < deadZone)
                {
                    accumulatedRotation = 0f;
                    isRotating = false;
                }
            }
        }
        else
        {
            // Durante el cooldown, no acumular rotación
            accumulatedRotation = 0f;
            isRotating = false;
        }
    }
}