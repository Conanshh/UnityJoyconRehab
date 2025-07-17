using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona el spawn de objetos en posiciones fijas definidas para obstaculos y recompensas.
/// Permite obtener una posición disponible para generar un objeto y liberar esa posición al destruirse el objeto.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    // Posiciones fijas en X donde se pueden generar objetos.
    private readonly float[] spawnPositionsX = { -0.254f, 0.26f, 0.78f };

    public float spawnY = 0.79f; // Ajusta la altura donde aparecen los objetos
    private float spawnZ = 0f;   // Ajusta la profundidad donde aparecen los objetos

    // Posiciones fijas en X donde se pueden generar objetos.
    private HashSet<int> occupiedPositionsObstacles = new HashSet<int>(); 
    private HashSet<int> occupiedPositionsRewards = new HashSet<int>();

    // Historial de los últimos índices usados para evitar repeticiones.
    private Queue<int> lastUsedIndices = new Queue<int>(); 
    private const int historySize = 2; // Evitar repetir posiciones de los últimos 2 spawns

    private bool lastRewardWasLeft; // Control para alternar recompensas
    private bool isPaused = false;          // Control para detener o renaudar el SpawnManager

    void Start()
    {
        // El primer reward será aleatorio entre carril 0 y 2
        lastRewardWasLeft = Random.value > 0.5f;
    }

    /// <summary>
    /// Pausa el funcionamiento del SpawnManager.
    /// </summary>
    public void Pause()
    {
        isPaused = true;
    }

    /// <summary>
    /// Reanuda el funcionamiento del SpawnManager.
    /// </summary>
    public void Resume()
    {
        isPaused = false;
    }

    /// <summary>
    /// Obtiene una posición disponible para generar un objeto.
    /// </summary>
    /// <param name="isObstacle">Indica si se solicita la posición para un enemigo (true) o para una recompensa (false).</param>
    /// <returns>La posición disponible en forma de Vector3 o null si no hay disponibles.</returns>
    public Vector3? GetAvailablePosition(bool isObstacle)
    {
        if (isPaused) return null; // Si está pausado, no hace nada

        List<int> availableIndices = new List<int>();

        // Recorrer todas las posiciones definidas
        for (int i = 0; i < spawnPositionsX.Length; i++)
        {
            bool occupied = isObstacle ? occupiedPositionsObstacles.Contains(i) : occupiedPositionsRewards.Contains(i);
            if (!occupied && !lastUsedIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0) return null;

        int chosenIndex;

        if (isObstacle)
        {
            // Para enemigos se elige aleatoriamente una de las posiciones disponibles.
            chosenIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        }
        else
        {
            // Para recompensas se alterna entre las posiciones izquierda (índice 0) y derecha (índice 2).
            chosenIndex = lastRewardWasLeft ? 2 : 0;

            // Si la posición elegida no está disponible, se elige una aleatoria.
            if (!availableIndices.Contains(chosenIndex))
            {
                chosenIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            }

            lastRewardWasLeft = !lastRewardWasLeft; // Alternar para la próxima recompensa.
        }

        // Actualizar historial para evitar repetición de posiciones.
        if (lastUsedIndices.Count >= historySize)
        {
            lastUsedIndices.Dequeue();
        }
        lastUsedIndices.Enqueue(chosenIndex);

        // Marcar la posición como ocupada según el tipo de objeto.
        if (isObstacle)
        {
            occupiedPositionsObstacles.Add(chosenIndex);
        }
        else
        {
            occupiedPositionsRewards.Add(chosenIndex);
        }

        return new Vector3(spawnPositionsX[chosenIndex], spawnY, spawnZ);
    }

    /// <summary>
    /// Libera una posición ocupada cuando se destruye un objeto.
    /// </summary>
    /// <param name="position">La posición a liberar.</param>
    /// <param name="isObstacle">Indica si la posición pertenece a un enemigo (true) o a una recompensa (false).</param>    
    public void ReleasePosition(Vector3 position, bool isObstacle)
    {
        for (int i = 0; i < spawnPositionsX.Length; i++)
        {
            if (Mathf.Approximately(position.x, spawnPositionsX[i]))
            {
                if (isObstacle)
                {
                    occupiedPositionsObstacles.Remove(i);
                }
                else
                {
                    occupiedPositionsRewards.Remove(i);
                }
                break;
            }
        }
    }
}
