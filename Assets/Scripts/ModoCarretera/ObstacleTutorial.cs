using UnityEngine;
using TMPro;

/// <summary>
/// Comportamiento para un obstáculo en el modo tutorial. 
/// Se mueve hacia abajo a una velocidad constante y, al colisionar con el Player,
/// notifica al jugador para que gestione la colisión antes de autodestruirse.
/// </summary>
public class ObstacleTutorial : MonoBehaviour
{
    /// <summary>
    /// Velocidad de caída del obstáculo.
    /// </summary>
    public float fallSpeed = 0.8f;

    /// <summary>
    /// Actualiza la posición del objeto descendiendo de forma constante.
    /// </summary>
    void FixedUpdate()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Detecta cuando colisiona con el Player y, en ese caso, se encarga de notificar al jugador y destruirse.
    /// </summary>
    /// <param name="other">Collider2D con el que se ha colisionado.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerRoad"))
        {
            other.GetComponent<PlayerRoad>().HandleCollisionTutorial();
            Destroy(gameObject);
        }
    }
}
