using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Gestiona la interfaz de usuario para la selección y creación de usuarios.
/// Permite cargar, seleccionar y crear usuarios, actualizando el dropdown y notificando al menú principal.
/// </summary>
public class UserUIManager : MonoBehaviour
{
    [Header("Controles del Usuario")]
    [SerializeField] private TMP_Dropdown userDropdown;
    [SerializeField] private TMP_InputField newUserInput;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button BackButton;

    [Header("Paneles")]
    public GameObject panelUser;
    public GameObject mainMenuPanel;

    [Header("Script de Menú Principal")]
    [SerializeField] private MainMenu mainMenuScript;

    // Lista local de usuarios (se actualiza al cargar desde UserManager).
    private List<string> users = new List<string>();

    void Start()
    {
        LoadUsers();
        selectButton.onClick.AddListener(SetSelectedUser);
        createButton.onClick.AddListener(CreateNewUser);
        BackButton.onClick.AddListener(BackGame);
        // Agrega sonido al cambiar la opción del dropdown.
        userDropdown.onValueChanged.AddListener(delegate {
            AudioManager.Instance.PlayClickSound();
        });
    }

    /// <summary>
    /// Carga la lista de usuarios desde el UserManager y actualiza el dropdown.
    /// Si hay un usuario actual, lo coloca al inicio.
    /// </summary>
    void LoadUsers()
    {
        List<string> allUsers = UserManager.Instance.GetUsers();
        string currentUser = UserManager.Instance.GetCurrentUser();

        // Si existe un usuario actual y está en la lista, se mueve al inicio.
        if (!string.IsNullOrEmpty(currentUser) && allUsers.Contains(currentUser))
        {
            allUsers.Remove(currentUser);
            allUsers.Insert(0, currentUser);
        }

        userDropdown.ClearOptions();
        userDropdown.AddOptions(allUsers);

        // Seleccionar automáticamente el usuario actual (si existe) en el dropdown.
        if (!string.IsNullOrEmpty(currentUser))
            userDropdown.value = 0;
    }

    /// <summary>
    /// Establece el usuario seleccionado en el dropdown y notifica al MainMenu.
    /// </summary>
    void SetSelectedUser()
    {
        string selected = userDropdown.options[userDropdown.value].text;
        if (UserManager.Instance.GetCurrentUser() == selected)
        {
            Debug.LogWarning("El usuario ya está seleccionado.");
            return; // No se realiza ningún cambio.
        }
        UserManager.Instance.SetUser(selected);
        mainMenuScript.EnableStartButton();
        AudioManager.Instance.PlayClickSound();
    }

    /// <summary>
    /// Crea un nuevo usuario si el nombre ingresado es válido y no existe ya.
    /// Luego actualiza el dropdown para incluir el nuevo usuario.
    /// </summary>
    void CreateNewUser()
    {
        string newUser = newUserInput.text.Trim();
        if (!string.IsNullOrEmpty(newUser) && !users.Contains(newUser))
        {
            AudioManager.Instance.PlayClickSound();
            UserManager.Instance.SetUser(newUser);
            mainMenuScript.EnableStartButton();
            LoadUsers(); // Recarga la lista para mostrar el usuario creado.
        }
        else
        {
            AudioManager.Instance.PlayClickSound();
            Debug.LogWarning("Nombre vacío o ya existente.");
        }
    }

    /// <summary>
    /// Regresa al menú principal ocultando el panel de usuarios y mostrando el panel principal.
    /// </summary>
    void BackGame()
    {
        AudioManager.Instance.PlayClickSound();
        panelUser.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
