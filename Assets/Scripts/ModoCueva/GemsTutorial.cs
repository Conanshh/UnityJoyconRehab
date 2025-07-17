using UnityEngine;

/// <summary>
/// Representa una gema especial para el tutorial.
/// Se encarga de detectar colisiones con el jugador, reproducir el sonido de recolección,
/// notificar al jugador para incrementar la cantidad de gemas y autodestruirse.
/// La lógica se simplifica para el flujo guiado del tutorial.
/// </summary>
public class GemsTurorial : MonoBehaviour
{
    /// <summary>
    /// Representa una gema especial para el tutorial.
    /// Se encarga de detectar colisiones con el jugador, reproducir el sonido de recolección,
    /// notificar al jugador para incrementar la cantidad de gemas y autodestruirse.
    /// La lógica se simplifica para el flujo guiado del tutorial.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCave"))
        {
            AudioManager.Instance.PlayCollisionGem();
            // Llamar a la función del PlayerCave para que maneje la desactivación y reactivación
            //other.GetComponent<PlayerCave>().AddGem();
            // Destruir la gema tutorial después de ser recolectada
            Destroy(gameObject);
        }
    }
}
