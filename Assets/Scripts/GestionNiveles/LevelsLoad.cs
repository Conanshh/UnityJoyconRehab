using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la navegación entre escenas de niveles, permitiendo cargar distintos modos o volver al menú principal.
/// Ussado en la escena de selección de niveles.
/// </summary>
public class LevelsLoad : MonoBehaviour
{
    /// <summary>
    /// Carga la escena del nivel Carretera.
    /// </summary>
    public void LoadCarretera()
    {
        SceneManager.LoadScene("PreLevelAbAd");
    }

    /// <summary>
    /// Carga la escena del nivel Cueva.
    /// </summary>
    public void LoadCueva()
    {
        SceneManager.LoadScene("PreLevelFE");
    }

    /// <summary>
    /// Retorna al menú principal.
    /// </summary>
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
