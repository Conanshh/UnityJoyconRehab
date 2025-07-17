using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la navegaci�n previa al nivel de Abducci�n/Aducci�n (Carretera).
/// Permite iniciar el juego, el tutorial, la selecci�n de personaje y los ajustes.
/// </summary>
public class PrelevelAbAd : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject characterPanel;

    /// <summary>
    /// Inicia el nivel de Abducci�n/Aducci�n.
    /// </summary>
    public void StartGame()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("LevelAbAd");
    }

    /// <summary>
    /// Inicia el tutorial de Abducci�n/Aducci�n.
    /// </summary>
    public void StartTutorial()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("TutorialAbAd");
    }


    /// <summary>
    /// Muestra el panel de selecci�n de personaje y oculta el men� principal.
    /// Adem�s, carga los IDs de personajes almacenados para el usuario actual.
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
    /// Muestra el panel de ajustes y oculta el men� principal.
    /// </summary>
    public void StartAdjustments()
    {
        AudioManager.Instance.PlayClickSound();
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Regresa al men� de niveles.
    /// </summary>
    public void Back()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("Levels");
    }
}
