using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la generación de recompensas (monedas u otros premios) durante el juego Carretera.
/// Controla la aparición de recompensas a intervalos variables y su destrucción tras un tiempo determinado.
/// </summary>
public class RewardSpawner : MonoBehaviour
{
    [Header("Prefabs de Recompensas")]
    public GameObject[] rewardPrefabs; // Prefabs de recompensas (monedas u otros premios)

    [Header("Gestión de Recompensas")]
    private List<GameObject> activeCoins = new List<GameObject>(); // Lista de monedas activas
    public SpawnManager spawnManager;

    [Header("Configuración de Spawn")]
    public float initialSpawnTime = 3f; // Por ahora, 3s entre spawns es útil pra no tener que repetir movimeintos
    private float spawnTimer;
    public float spawnY = 0.79f;         // Altura donde aparecen las recompensas
    public float despawnTime = 4f;    // Tiempo antes de destruir las recompensas

    private bool isSpawning = true; // Indicador para controlar el spawn de recompensas

    private Road gameTimer; // Referencia al script Road para conocer el tiempo restante del juego

    void Start()
    {
        spawnTimer = initialSpawnTime;
        gameTimer = FindFirstObjectByType<Road>();
        StartCoroutine(SpawnRewards());
    }

    /// <summary>
    /// Corrutina que genera recompensas mientras isSpawning sea verdadero.
    /// </summary>
    IEnumerator SpawnRewards()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnTimer);
            SpawnReward(); 
            AdjustSpawnRate();
        }
    }

    /// <summary>
    /// Ajusta el intervalo de spawn en función del tiempo restante del juego.
    /// </summary>
    void AdjustSpawnRate()
    {
        if (gameTimer == null) return;

        float remainingTime = gameTimer.gameTime;
        if (remainingTime <= 30f)
        {
            spawnTimer = 1.5f;
        }
    }

    /// <summary>
    /// Genera una recompensa en una posición disponible obtenida del SpawnManager.
    /// </summary>
    void SpawnReward()
    {
        // Obtener posición libre desde el SpawnManager para recompensas (false indica recompensa)
        Vector3? spawnPosition = spawnManager.GetAvailablePosition(false); 
        if (spawnPosition.HasValue)
        {
            // Seleccionar aleatoriamente un prefab de recompensa
            GameObject selectedPrefab = rewardPrefabs[Random.Range(0, rewardPrefabs.Length)];
            GameObject newCoin = Instantiate(selectedPrefab, spawnPosition.Value, Quaternion.identity);
            activeCoins.Add(newCoin);

            // Configurar la recompensa para que libere su posición al destruirse
            Reward rewardScript = newCoin.GetComponent<Reward>();
            if (rewardScript != null)
            {
                rewardScript.SetSpawnManager(spawnManager);
            }

            // Destruir la recompensa después de un tiempo determinado
            Destroy(newCoin, despawnTime);
        }
    }

    /// <summary>
    /// Destruye todas las recompensas activas y limpia la lista.
    /// </summary>
    public void DestroyAllCoins()
    {
        foreach (GameObject coin in activeCoins)
        {
            if (coin != null)
            {
                Destroy(coin);
            }
        }
        activeCoins.Clear();
    }

    /// <summary>
    /// Detiene la generación de recompensas.
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
    }

    /// <summary>
    /// Inicia (o reinicia) la generación de recompensas.
    /// </summary>
    public void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnRewards());
    }
}
