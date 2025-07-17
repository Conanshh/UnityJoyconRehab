using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// Inicia el efecto de sacudida de la cámara.
    /// </summary>
    /// <param name="duration">Duración total de la sacudida en segundos.</param>
    /// <param name="magnitude">Magnitud del desplazamiento aplicado a la cámara.</param>
    public void Shake(float duration, float magnitude)
    {
        // Se inicia la corrutina que aplica el efecto de sacudida.
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    /// <summary>
    /// Corrutina que aplica un efecto de sacudida a la cámara modificando su posición local.
    /// Durante el tiempo especificado, se genera una posición aleatoria dentro de un rango para simular el movimiento.
    /// </summary>
    /// <param name="duration">Duración total del efecto de sacudida en segundos.</param>
    /// <param name="magnitude">Magnitud del desplazamiento aplicado a la cámara.</param>
    /// <returns>IEnumerator para controlar la corrutina.</returns>
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        // Se guarda la posición original de la cámara para restaurarla al finalizar el efecto.
        Vector3 originalPos = transform.localPosition;

        // Variable para acumular el tiempo transcurrido.
        float elapsed = 0.0f;

        // Mientras no se haya alcanzado la duración de la sacudida...
        while (elapsed < duration)
        {
            // Se calcula un desplazamiento aleatorio en el eje X y Y usando Random.Range.
            // Se multiplica por la magnitud y por un factor de ajuste (0.1f) para suavizar el efecto.
            float offsetX = Random.Range(-1f, 1f) * magnitude * 0.1f;
            float offsetY = Random.Range(-1f, 1f) * magnitude * 0.1f;

            // Se aplica el desplazamiento sumándolo a la posición original.
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            // Se incrementa el tiempo transcurrido.
            elapsed += Time.deltaTime;

            // Se espera el siguiente frame.
            yield return null;
        }

        // Al finalizar, se restaura la posición original de la cámara.
        transform.localPosition = originalPos;
    }
}