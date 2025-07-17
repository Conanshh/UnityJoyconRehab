using UnityEngine;

/// <summary>
/// Gestiona el comportamiento de la recompensa en el tutorial.
/// La recompensa se desplaza hacia abajo a una velocidad definida.
/// </summary>
public class RewardTutorial : MonoBehaviour
{
    [Header("Configuraci�n de la Recompensa")]
    public float fallSpeed = 0.8f; // Velocidad de ca�da del Reward

    void Update()
    {
        // Mover el Reward hacia abajo a la velocidad definida
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
}
