using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla el menú principal del juego, gestionando la navegación a distintas secciones (niveles, ajustes, usuario)
/// y la disponibilidad del botón de inicio en función del usuario actual.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelajustes;     // Panel de ajustes.
    public GameObject mainMenuPanel;    // Panel principal del menú.
    public GameObject panelUser;        // Panel de usuario.

    [Header("Botones")]
    [SerializeField] private Button startGameButton; // Botón para iniciar el juego.

    /// <summary>
    /// Inicializa el menú principal configurando el estado del botón de inicio dependiendo de si 
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
            Debug.LogWarning("startGameButton no está asignado en el Inspector.");
        }
    }

    /// <summary>
    /// Habilita el botón de inicio del juego.
    /// </summary>
    public void EnableStartButton()
    {
        startGameButton.interactable = true;
    }

    /// <summary>
    /// Carga la escena de selección de niveles.
    /// Determina si se debe saltar el tutorial o mostrarlo según el estado del usuario.
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
    /// Muestra el panel de usuario, ocultando el panel principal del menú.
    /// </summary>
    public void LoadUserPanel()
    {
        AudioManager.Instance.PlayClickSound();
        mainMenuPanel.SetActive(false);
        panelUser.SetActive(true);
    }

    /// <summary>
    /// Muestra el panel de ajustes, ocultando el panel principal del menú.
    /// </summary>
    public void LoadAdjust()
    {
        AudioManager.Instance.PlayClickSound();
        mainMenuPanel.SetActive(false);
        panelajustes.SetActive(true);
    }

    /// <summary>
    /// Finaliza la aplicación.
    /// </summary>
    public void QuitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }
}
