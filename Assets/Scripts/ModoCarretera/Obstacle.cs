using UnityEngine;

/// <summary>
/// Clase que representa un obstáculo en el nivel (por ejemplo, autos, rocas u otros objetos).
/// Se encarga de mover el objeto y notificar al SpawnManager cuando se destruye.
/// </summary>
public class Obstacle : MonoBehaviour
{
    // Velocidad de movimiento hacia abajo del obstáculo.
    public float fallSpeed = 0.8f;

    // Referencia al gestor de spawn para notificar la liberación de la posición.
    private SpawnManager spawnManager;

    /// <summary>
    /// Asigna el SpawnManager que controla las posiciones de aparición.
    /// </summary>
    /// <param name="manager">Instancia del SpawnManager.</param>
    public void SetSpawnManager(SpawnManager manager)
    {
        spawnManager = manager;
    }

    /// <summary>
    /// Actualiza la posición del obstáculo, moviéndolo hacia abajo.
    /// Se utiliza FixedUpdate para un movimiento consistente.
    /// </summary>
    void FixedUpdate()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Al destruir el obstáculo, se notifica al SpawnManager que libera la posición ocupada.
    /// </summary>
    void OnDestroy()
    {
        if (spawnManager != null)
        {
            // El segundo parámetro indica si es un obstáculo (true) o una recompensa (false)
            spawnManager.ReleasePosition(transform.position, true);
        }
    }
}
