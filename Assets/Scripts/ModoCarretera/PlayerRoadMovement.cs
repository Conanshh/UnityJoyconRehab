using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el movimiento lateral del jugador en la modalidad Carretera, 
/// utilizando los datos del giroscopio del Joy-Con para detectar inclinaciones y cambiar de carril.
/// </summary>
public class PlayerRoadMovement : MonoBehaviour
{
    // Lista de Joy-Cons conectados.
    private List<Joycon> joycons;

    // Índice del Joy-Con seleccionado.
    private int jc_ind = 0;

    // Valor inicial del giroscopio en Z al iniciar.
    private float initialGyroZ;

    // Valor suavizado del giroscopio en Z.
    private float smoothedGyroZ = 0f;

    // Factor de suavizado para la lectura del giroscopio (más bajo = más suave).
    [Range(0.01f, 0.2f)]
    public float gyroSmoothingFactor = 0.09f;

    // Zona muerta para ignorar pequeños movimientos del giroscopio.
    public float deadZone = 0.3f;

    // Tiempo mínimo (en segundos) entre cambios de carril.
    public float laneChangeCooldown = 0.15f;

    // Marca de tiempo del último cambio de carril.
    private float lastLaneChangeTime = -1f;

    // Umbral de rotación (en grados) necesario para disparar el cambio de carril.
    public float rotationThresholdDeg = 30f;

    // Acumulador del ángulo rotado desde el último cambio de carril.
    private float accumulatedRotation = 0f;

    // Indica si actualmente se está detectando un movimiento de rotación.
    private bool isRotating = false;

    // Indica si se utiliza el Joy-Con izquierdo.
    public bool isLeftJoycon;

    // Carril actual del jugador (0: izquierda, 1: centro, 2: derecha).    
    public int currentLane = 1;

    // Posiciones X predefinidas para cada carril.
    public float[] lanePositions = { -0.254f, 0.26f, 0.78f };

    private PlayerRoad playerRoad;

    //public event Action<int, string> OnLaneChanged;
    public event Action OnCollisionDetected;
    public event Action OnCoinDetected;

    void Start()
    {
        playerRoad = GetComponent<PlayerRoad>();
        GameData gameData = GameData.Load();
        rotationThresholdDeg = gameData.thresholdAngleAbAd;
        isLeftJoycon = gameData.isLeftJoyCon;
        joycons = JoyconManager.Instance.j;
        UpdateJoyconIndex();
        if (joycons.Count > jc_ind)
            initialGyroZ = joycons[jc_ind].GetGyro().z;
        smoothedGyroZ = initialGyroZ;
        MovementLogger.Instance.SetSessionInfo(isFE: false, thresholdAngle: rotationThresholdDeg, axis: "Gyro Z", gameTime: gameData.GameTime, isLeftJoycon);
        CenterLane();
    }

    /// <summary>
    /// Centra al jugador en el carril 1.
    /// </summary>
    public void CenterLane()
    {
        currentLane = 1;
        transform.position = new Vector3(lanePositions[currentLane], transform.position.y, transform.position.z);
    }

    /// <summary>
    /// Actualiza el índice del Joy-Con a utilizar basándose en la preferencia (izquierdo o derecho).
    /// </summary>
    public void UpdateJoyconIndex()
    {
        GameData gameData = GameData.Load();
        isLeftJoycon = gameData.isLeftJoyCon;
        for (int i = 0; i < joycons.Count; i++)
        {
            if (joycons[i].isLeft == isLeftJoycon)
            {
                jc_ind = i;
                initialGyroZ = joycons[jc_ind].GetGyro().z;
                smoothedGyroZ = initialGyroZ;
                break;
            }
        }
    }

    /// <summary>
    ///  Metodo para reiniciar el estado de movimiento del jugador, reseteando la acumulación de rotación y el estado de rotación.
    /// </summary>
    public void ResetMovementState()
    {
        accumulatedRotation = 0f;
        isRotating = false;
    }

    /// <summary>
    /// Metodo para reiniciar el giroscopio del Joy-Con, estableciendo el valor suavizado en el valor actual del giroscopio.
    /// </summary>
    public void ResetGyro()
    {
        if (joycons != null && joycons.Count > jc_ind)
        {
            smoothedGyroZ = joycons[jc_ind].GetGyro().z;
        }
    }

    /// <summary>
    /// Lógica de movimiento del jugador basada en el giroscopio, aplicando filtro, zona muerta y cooldown.
    /// </summary>
    void FixedUpdate()
    {
        if (joycons == null || joycons.Count == 0) return;

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
                        //MovementLogger.Instance.RegisterAbductionAdductionMovement(
                        //    moveType, accumulatedRotation, rotationThresholdDeg);
                        if (!SceneManager.GetActiveScene().name.ToLower().Contains("tutorial") && playerRoad != null)
                        {
                            // Solo registrar si NO es la escena de tutorial
                            playerRoad.RegistrarMovimientoAbduccionAduccion(moveType, accumulatedRotation, rotationThresholdDeg);
                        }
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


    /// <summary>
    /// Detecta colisiones con obstáculos y monedas, notificando los eventos correspondientes.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            OnCollisionDetected?.Invoke();
            AudioManager.Instance.PlayCollisionObstacle();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Coin"))
        {
            OnCoinDetected?.Invoke();
            AudioManager.Instance.PlayCollisionCoin();
            Destroy(other.gameObject);
        }
    }
}