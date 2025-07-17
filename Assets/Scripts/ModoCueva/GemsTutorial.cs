using UnityEngine;

/// <summary>
/// Representa una gema especial para el tutorial.
/// Se encarga de detectar colisiones con el jugador, reproducir el sonido de recolecci�n,
/// notificar al jugador para incrementar la cantidad de gemas y autodestruirse.
/// La l�gica se simplifica para el flujo guiado del tutorial.
/// </summary>
public class GemsTurorial : MonoBehaviour
{
    /// <summary>
    /// Representa una gema especial para el tutorial.
    /// Se encarga de detectar colisiones con el jugador, reproducir el sonido de recolecci�n,
    /// notificar al jugador para incrementar la cantidad de gemas y autodestruirse.
    /// La l�gica se simplifica para el flujo guiado del tutorial.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCave"))
        {
            AudioManager.Instance.PlayCollisionGem();
            // Llamar a la funci�n del PlayerCave para que maneje la desactivaci�n y reactivaci�n
            //other.GetComponent<PlayerCave>().AddGem();
            // Destruir la gema tutorial despu�s de ser recolectada
            Destroy(gameObject);
        }
    }
}
