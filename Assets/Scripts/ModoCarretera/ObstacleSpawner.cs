using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase responsable de gestionar la generación dinámica de obstáculos en el nivel.
/// Se encarga de instanciar prefabs de obstáculos en posiciones disponibles definidas por el SpawnManager,
/// ajustar la tasa de aparición según el tiempo restante del juego y gestionar la destrucción de estos objetos.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    // Lista de prefabs de obstáculos disponibles para instanciar.
    public GameObject[] obstaclePrefabs;

    // Referencia al administrador de posiciones de spawn.
    public SpawnManager spawnManager;

    // Lista de obstáculos activos en la escena.
    private List<GameObject> activeObstacles = new List<GameObject>(); private List<GameObject> activeEnemys = new List<GameObject>();

    // Temporizador que regula el intervalo entre cada spawn.
    private float spawnTimer;

    // Referencia al temporizador del juego que permite ajustar la tasa de spawn.
    private Road gameTimer;

    // Intervalo inicial entre cada aparición de un obstáculo.    
    public float initialSpawnTime = 2f;

    // Altura en la que aparecen los obstáculos.
    public float spawnY = 0.79f;

    // Tiempo transcurrido antes de destruir un obstáculo.
    public float despawnTime = 5f;

    // Posiciones fijas en X para la aparición de obstáculos.
    private readonly float[] spawnPositionsX = { -0.254f, 0.26f, 0.78f };

    // Bandera que determina si el spawner sigue generando obstáculos.
    private bool isSpawning = true;

    /// <summary>
    /// Inicializa el spawner, configura el temporizador y comienza la generación de obstáculos.
    /// </summary>
    void Start()
    {
        spawnTimer = initialSpawnTime;
        gameTimer = FindFirstObjectByType<Road>();
        if (gameTimer == null)
        {
            Debug.LogWarning($"ObstacleSpawner recibió gameTime nulo: {gameTimer.gameTime}");
            return;
        }
        StartCoroutine(SpawnObstacles());
    }

    /// <summary>
    /// Corrutina que genera obstáculos de forma periódica mientras el spawner esté activo.
    /// </summary>
    /// <returns>IEnumerator para la ejecución de la corrutina.</returns>
    IEnumerator SpawnObstacles()
    {
        int spawnCount = 0; // Contador de obstáculos generados

        while (isSpawning)
        {            
            yield return new WaitForSeconds(spawnTimer);
            SpawnObstacle();
            spawnCount++; // Se incrementa el contador de obstáculos generados
            AdjustSpawnRate();
        }
    }

    /// <summary>
    /// Ajusta dinámicamente la tasa de spawn de obstáculos en función del tiempo restante del juego.
    /// Si el tiempo restante es menor o igual a 30 segundos, se establece un intervalo fijo de 1 segundo.
    /// </summary>
    void AdjustSpawnRate()
    {
        if (gameTimer == null)
        {
            Debug.LogWarning("No se encontró `Road`, no se ajustará el spawn rate.");
            return;
        }

        float remainingTime = gameTimer.gameTime; // Obtener tiempo restante
        if (remainingTime <= 30f) // Cuando el tiempo sea <= 30 seg
        {
            spawnTimer = 1.0f; // Se ajusta el spawn a 1 segundo
        }
    }

    /// <summary>
    /// Instancia un obstáculo en una posición disponible determinada por el SpawnManager.
    /// Selecciona aleatoriamente un prefab de la lista, reproduce el sonido de spawn, asigna el SpawnManager y
    /// programa su destrucción.
    /// </summary>
    void SpawnObstacle()
    {
        // Obtener una posición libre para obstáculo (true indica que se reserva para obstáculos).
        Vector3? spawnPosition = spawnManager.GetAvailablePosition(true);
        if (spawnPosition.HasValue)
        {
            // Elegir un prefab aleatorio.
            GameObject selectedPrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]; // 🔹 Elegir un prefab aleatorio
            GameObject newObstacle = Instantiate(selectedPrefab, spawnPosition.Value, Quaternion.identity);

            AudioManager.Instance.PlaySpawnObstacle();
            activeObstacles.Add(newObstacle);

            // Asignar el SpawnManager al componente Obstacle si está presente.
            Obstacle obstacleScript = newObstacle.GetComponent<Obstacle>();
            if (obstacleScript != null)
            {
                obstacleScript.SetSpawnManager(spawnManager);
            }
            // Programar la destrucción del obstáculo después de un tiempo determinado.
            Destroy(newObstacle, despawnTime);
        }
    }

    /// <summary>
    /// Destruye todos los obstáculos activos y libera los recursos asociados.
    /// </summary>
    public void DestroyAllObstacles()
    {
        // Limpia referencias nulas
        activeObstacles.RemoveAll(item => item == null);
        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
                Resources.UnloadUnusedAssets();
            }
        }
        activeObstacles.Clear(); // Limpiar la lista
    }

    /// <summary>
    /// Pausa la generación de obstáculos.
    /// </summary>
    public void PauseSpawner()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// Reanuda la generación de obstáculos e inicia nuevamente la corrutina.
    /// </summary>
    public void ResumeSpawner()
    {
       isSpawning = true;
       StartCoroutine(SpawnObstacles());
    }
}
