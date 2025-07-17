using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Muestra el efecto de carga en la primera escena y luego transita a la escena principal.
/// No se encarga de inicializar managers globales, ya que estos son iniciados directamente desde Unity.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    private string baseText = "Iniciando";

    void Start()
    {
        // Inicia la animación de los puntos
        StartCoroutine(AnimateDots()); 
        StartCoroutine(LoadScene());
    }

    /// <summary>
    /// Coroutine que anima los puntos en el texto de carga, creando un efecto de "cargando...".
    /// </summary>
    private IEnumerator AnimateDots()
    {
        int dotCount = 0;
        while (true) 
        {
            dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3 (reinicia en 0)
            loadingText.text = baseText + new string('.', dotCount); // Cambia los puntos
            yield return new WaitForSeconds(0.5f); // Intervalo de cambio
        }
    }

    /// <summary>
    /// Coroutine que espera un tiempo determinado antes de cargar la escena principal.
    /// </summary>
    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1.5f);
        // Carga la escena principal del juego
        SceneManager.LoadScene("MainMenu");
    }
}
