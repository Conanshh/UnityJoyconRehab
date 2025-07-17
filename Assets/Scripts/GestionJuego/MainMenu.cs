using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla el men� principal del juego, gestionando la navegaci�n a distintas secciones (niveles, ajustes, usuario)
/// y la disponibilidad del bot�n de inicio en funci�n del usuario actual.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Paneles del Men�")]
    public GameObject panelajustes;     // Panel de ajustes.
    public GameObject mainMenuPanel;    // Panel principal del men�.
    public GameObject panelUser;        // Panel de usuario.

    [Header("Botones")]
    [SerializeField] private Button startGameButton; // Bot�n para iniciar el juego.

    /// <summary>
    /// Inicializa el men� principal configurando el estado del bot�n de inicio dependiendo de si 
    /// hay un usuario actual registrado.
    /// </summary>
    void Start()
    {
        if (startGameButton != null)
        {
            string currentUser = UserManager.Instance?.GetCurrentUser();
            startGameButton.interactable = !string.IsNullOrEmpty(currentUser);
        }
        else
        {
            Debug.LogWarning("startGameButton no est� asignado en el Inspector.");
        }
    }

    /// <summary>
    /// Habilita el bot�n de inicio del juego.
    /// </summary>
    public void EnableStartButton()
    {
        startGameButton.interactable = true;
    }

    /// <summary>
    /// Carga la escena de selecci�n de niveles.
    /// Determina si se debe saltar el tutorial o mostrarlo seg�n el estado del usuario.
    /// </summary>
    public void LoadLevels()
    {
        string currentUser = UserManager.Instance?.GetCurrentUser();
        AudioManager.Instance.PlayStartGameSound();
        if (UserManager.Instance.HasSeenTutorial(currentUser))
        {
            SceneManager.LoadScene("Levels"); // Si el tutorial ya fue visto, carga directamente los niveles.
        }
        else
        {
            SceneManager.LoadScene("TutorialJoyCon");  // De lo contrario, muestra el tutorial.
        }
    }

    /// <summary>
    /// Muestra el panel de usuario, ocultando el panel principal del men�.
    /// </summary>
    public void LoadUserPanel()
    {
        AudioManager.Instance.PlayClickSound();
        mainMenuPanel.SetActive(false);
        panelUser.SetActive(true);
    }

    /// <summary>
    /// Muestra el panel de ajustes, ocultando el panel principal del men�.
    /// </summary>
    public void LoadAdjust()
    {
        AudioManager.Instance.PlayClickSound();
        mainMenuPanel.SetActive(false);
        panelajustes.SetActive(true);
    }

    /// <summary>
    /// Finaliza la aplicaci�n.
    /// </summary>
    public void QuitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }
}
