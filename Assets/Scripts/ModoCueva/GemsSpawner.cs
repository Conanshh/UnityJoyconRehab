using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se encarga de gestionar el spawn de gemas en posiciones predefinidas.  
/// La clase controla dos posiciones (arriba y abajo) y evita el spawn si el jugador se encuentra en alguna de ellas.  
/// Además, registra los índices de gemas recientemente spawneadas para evitar repeticiones inmediatas.
/// </summary>
public class GemsSpawner : MonoBehaviour
{
    [Header("Prefabs de gemas")]
    public List<GameObject> gemPrefabs;

    [Header("Posiciones posibles")]
    public float upperY = 0.19f;
    public float lowerY = -0.52f;
    public float spawnX = 0.296f;

    [Header("Configuración de tiempo")]
    public float spawnTimer = 3.4f;

    private float timer = 0f;

    // Control de posiciones ocupadas: 0 => arriba, 1 => abajo.
    private bool occupiedUp = false;
    private bool occupiedDown = false;

    // Indica la última posición usada: 0 (arriba), 1 (abajo) o -1 si aún no se ha usado.
    private int lastSpawnedIndex = -1;

    // Cola para almacenar los índices de gemas spawneadas recientemente.
    private Queue<int> lastGemIndices = new Queue<int>();

    /// <summary>
    /// Actualiza el temporizador y, cuando se alcanza el límite, invoca el spawn de una gema.
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnTimer)
        {
            timer = 0f;
            SpawnGem();
        }
    }

    /// <summary>
    /// Permite modificar el tiempo entre spawns, siendo llamado externamente (por ejemplo, desde otro script).
    /// </summary>
    /// <param name="newTimer">Nuevo intervalo de tiempo para realizar el spawn.</param>
    public void SetSpawnTimer(float newTimer)
    {
        spawnTimer = newTimer;
    }

    /// <summary>
    /// Realiza el spawn de una gema en la próxima posición disponible.  
    /// Verifica que el jugador no se encuentre en la posición de spawn y que la posición esté libre.  
    /// Selecciona un prefab basándose en la lógica de exclusión para evitar repeticiones inmediatas.
    /// </summary>
    void SpawnGem()
    {
        if (gemPrefabs.Count == 0) return;

        // Verificar si el jugador se encuentra en alguna de las posiciones de spawn.
        if (IsPlayerInPosition(upperY) || IsPlayerInPosition(lowerY))
        {
            return;
        }

        // Verificar si ambas posiciones están ocupadas.
        if (occupiedUp && occupiedDown)
        {
            return;
        }

        // Determinar la próxima posición disponible, evitando repetir la última posición usada.
        int yIndex = GetNextAvailableIndex();
        if (yIndex == -1)
        {
            return;
        }

        float yPos = yIndex == 0 ? upperY : lowerY;

        // Seleccionar un índice de gema considerando la lógica de exclusión.
        int gemIndex = GetNextGemIndex();
        GameObject gemPrefab = gemPrefabs[gemIndex];

        Vector3 spawnPosition = new Vector3(spawnX, yPos, 0);
        GameObject gem = Instantiate(gemPrefab, spawnPosition, Quaternion.identity);

        // Asignar el índice de posición a la gema para que pueda notificar al spawner al ser recolectada.
        Gems gemScript = gem.GetComponent<Gems>();
        if (gemScript != null)
        {
            gemScript.SetPositionIndex(yIndex);
        }

        // Marcar la posición como ocupada y actualizar el índice de la última posición utilizada.
        if (yIndex == 0)
        {
            occupiedUp = true;
        }
        else
        {
            occupiedDown = true;
        }
        // Actualizar el índice de la última posición usada
        lastSpawnedIndex = yIndex;

        // Registrar el tipo de gema spawneada
        lastGemIndices.Enqueue(gemIndex);
        if (lastGemIndices.Count > 2)
            lastGemIndices.Dequeue(); // Solo mantener las 2 últimas
    }

    /// <summary>
    /// Selecciona el siguiente índice de gema disponible evitando repetir inmediatamente las gemas spawneadas.
    /// </summary>
    /// <returns>Índice del prefab de gema a utilizar.</returns>
    int GetNextGemIndex()
    {
        List<int> candidates = new List<int>();
        for (int i = 0; i < gemPrefabs.Count; i++)
            candidates.Add(i);

        // Evitar repetir inmediatamente la última gema utilizada.
        if (lastGemIndices.Count >= 1)
        {
            candidates.Remove(lastGemIndices.Peek()); // última gema
        }

        // Si ya hay dos registradas, eliminar ambas de las opciones.
        if (lastGemIndices.Count >= 2)
        {
            int[] lastTwo = lastGemIndices.ToArray();
            foreach (int idx in lastTwo)
                candidates.Remove(idx);
        }

        if (candidates.Count == 0)
        {
            // Si no hay candidatos disponibles, reinicializar la lista y mantener la exclusión de la última.
            for (int i = 0; i < gemPrefabs.Count; i++)
                candidates.Add(i);

            if (lastGemIndices.Count > 0)
                candidates.Remove(lastGemIndices.Peek());
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    /// <summary>
    /// Libera la posición ocupada por una gema cuando ésta es recolectada.
    /// </summary>
    /// <param name="yIndex">Índice de la posición a liberar (0 para arriba, 1 para abajo).</param>    
    public void FreePosition(int yIndex)
    {
        if (yIndex == 0)
        {
            occupiedUp = false;
        }
        else if (yIndex == 1)
        {
            occupiedDown = false;
        }
    }

    /// <summary>
    /// Verifica si el jugador se encuentra en una posición específica en el eje Y.  
    /// Se asume que el jugador posee el tag "PlayerCave" y se compara su posición con la posición indicada.
    /// </summary>
    /// <param name="yPos">La posición Y a verificar.</param>
    /// <returns>true si el jugador se encuentra en la posición (dentro de un margen), false en caso contrario.</returns>
    bool IsPlayerInPosition(float yPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerCave");
        if (player != null)
        {
            float playerY = player.transform.position.y;
            if (Mathf.Abs(playerY - yPos) < 0.1f)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determina la próxima posición disponible para el spawn de una gema, evitando repetir la última posición utilizada cuando sea posible.
    /// </summary>
    /// <returns>0 para la posición superior, 1 para la inferior o -1 si ambas están ocupadas.</returns>
    int GetNextAvailableIndex()
    {
        // Si se usó la posición superior la última vez, se intenta usar la inferior.
        if (lastSpawnedIndex == 0)
        {
            if (!occupiedDown)
            {
                return 1;
            }
        }
        // Si se usó la posición inferior la última vez, se intenta usar la superior.
        else if (lastSpawnedIndex == 1)
        {
            if (!occupiedUp)
            {
                return 0;
            }
        }
        else
        {
            // Si no se ha usado ninguna posición o la posición anterior no impone restricción, se elige la primera disponible.
            if (!occupiedUp)
            {
                return 0; 
            }
            else if (!occupiedDown)
            {
                return 1;
            }
        }
        return -1;
    }
}
