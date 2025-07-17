using UnityEngine;

/// <summary>
/// Representa una gema recolectable.
/// Se encarga de detectar colisiones con el jugador, reproducir el sonido de recolección,
/// notificar a GemsSpawner para liberar la posición ocupada y autodestruirse.
/// </summary>
public class Gems : MonoBehaviour
{
    private GemsSpawner gemsSpawner;
    public int yIndex = -1; // Índice de posición asignado. -1 indica índice no asignado.

    /// <summary>
    /// Inicializa las referencias necesarias.
    /// Se busca el GemsSpawner presente en la escena.
    /// </summary>
    void Start()
    {
        gemsSpawner = FindFirstObjectByType<GemsSpawner>();
    }

    /// <summary>
    /// Detecta colisiones y, si es el jugador, reproduce el sonido de recolección,
    /// notifica al GemsSpawner para liberar la posición y destruye la gema.
    /// </summary>
    /// <param name="other">Collider del objeto que ingresa en contacto.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCave"))
        {
            AudioManager.Instance.PlayCollisionGem();
            if (gemsSpawner != null && yIndex != -1)
            {
                gemsSpawner.FreePosition(yIndex);
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Asigna un índice de posición a la gema.
    /// Este valor es utilizado por GemsSpawner para gestionar los espacios ocupados.
    /// </summary>
    /// <param name="index">Índice asignado.</param>
    public void SetPositionIndex(int index)
    {
        yIndex = index;
    }
}