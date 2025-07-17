using UnityEngine;

/// <summary>
/// Gestiona el comportamiento de una recompensa en el juego Carretera.
/// La recompensa se desplaza hacia abajo a una velocidad definida y, al destruirse,
/// libera su posición en el SpawnManager.
/// </summary>
public class Reward : MonoBehaviour
{
    public float fallSpeed = 0.8f;      // Velocidad de caída del Reward
    private SpawnManager spawnManager;  // Referencia al SpawnManager

    /// <summary>
    /// Asigna el SpawnManager para esta recompensa.
    /// </summary>
    /// <param name="manager">Instancia del SpawnManager.</param>
    public void SetSpawnManager(SpawnManager manager)
    {
        spawnManager = manager;
    }

    void Update()
    {
        // Mover la recompensa hacia abajo a la velocidad definida
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    void OnDestroy()
    {
        // Libera la posición en el SpawnManager al destruirse la recompensa
        if (spawnManager != null)
        {
            spawnManager.ReleasePosition(transform.position, false); // false: indica que es una recompensa
        }
    }
}
