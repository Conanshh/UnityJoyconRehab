using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la navegación previa al nivel de Flexión/Extensión (modo Cueva).
/// Permite iniciar el juego, el tutorial, la selección de personaje y los ajustes.
/// </summary>
public class PrelevelFE : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject characterPanel;

    /// <summary>
    /// Inicia el nivel de Flexión/Extensión.
    /// </summary>
    public void StartGame()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("LevelFE");
    }

    /// <summary>
    /// Inicia el tutorial de Flexión/Extensión.
    /// </summary>
    public void StartTutorial()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("TutorialFE");
    }

    /// <summary>
    /// Muestra el panel de selección de personaje y oculta el menú principal.
    /// Además, carga los IDs de personajes para el usuario actual.
    /// </summary>
    public void StartCharacterSelection()
    {
        AudioManager.Instance.PlayClickSound();

        string username = UserManager.Instance.GetCurrentUser();
        MovementLogger.Instance?.LoadLatestCharacterIDs(username);

        characterPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Muestra el panel de ajustes y oculta el menú principal.
    /// </summary>
    public void StartAdjustments()
    {
        AudioManager.Instance.PlayClickSound();
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Regresa al menú de niveles.
    /// </summary>
    public void Back()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("Levels");
    }
}
