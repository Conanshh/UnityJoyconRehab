using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Clase que encapsula una lista de nombres de usuario.
[System.Serializable]
public class UserList
{
    // Lista de nombres de usuario registrados.
    public List<string> users = new List<string>();
}

// Clase que almacena el estado de visualización del tutorial para cada usuario.
[System.Serializable]
public class TutorialStatus
{
    // Diccionario que asocia cada nombre de usuario con un valor booleano que indica si ya vio el tutorial (true = visto).
    public Dictionary<string, bool> userTutorialSeen = new Dictionary<string, bool>();
}

// Clase que gestiona la información del usuario y el estado del tutorial del JoyCon en el juego.
public class UserManager : MonoBehaviour
{
    public static UserManager Instance;
    public string currentUser;
    private string userFilePath;
    private string tutorialSeenPath;
    private TutorialStatus tutorialStatus = new TutorialStatus();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePaths();
            LoadTutorialStatus();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa las rutas para los archivos de usuario y estado del tutorial.
    /// </summary>
    private void InitializePaths()
    {
        userFilePath = Path.Combine(Application.persistentDataPath, "users.json");
        tutorialSeenPath = Path.Combine(Application.persistentDataPath, "tutorial_seen.json");
    }

    #region Gestión del Estado del Tutorial

    /// <summary>
    /// Carga el estado del tutorial desde un archivo JSON.
    /// </summary>
    private void LoadTutorialStatus()
    {
        if (File.Exists(tutorialSeenPath))
        {
            string json = File.ReadAllText(tutorialSeenPath);
            tutorialStatus = JsonUtility.FromJson<TutorialStatus>(json);
        }
    }

    /// <summary>
    /// Guarda el estado del tutorial en un archivo JSON.
    /// </summary>
    private void SaveTutorialStatus()
    {
        string json = JsonUtility.ToJson(tutorialStatus, true);
        File.WriteAllText(tutorialSeenPath, json);
    }

    /// <summary>
    /// Determina si el usuario dado ya ha visto el tutorial.
    /// </summary>
    public bool HasSeenTutorial(string username)
    {
        return tutorialStatus.userTutorialSeen.ContainsKey(username) && tutorialStatus.userTutorialSeen[username];
    }

    /// <summary>
    /// Marca el tutorial como visto para el usuario especificado y guarda el estado.
    /// </summary>
    public void MarkTutorialSeen(string username)
    {        
        if (!tutorialStatus.userTutorialSeen.ContainsKey(username))
            tutorialStatus.userTutorialSeen.Add(username, true);
        else
            tutorialStatus.userTutorialSeen[username] = true;
        SaveTutorialStatus();
    }

    #endregion

    #region Gestión de Usuarios

    /// <summary>
    /// Establece el usuario actual, actualiza el MovementLogger y guarda el usuario.
    /// </summary>
    public void SetUser(string name)
    {
        currentUser = name;
        MovementLogger.Instance?.SetUser(name);
        SaveUser(name);
    }

    /// <summary>
    /// Retorna la lista de usuarios guardados.
    /// </summary>
    public List<string> GetUsers()
    {
        if (!File.Exists(userFilePath)) return new List<string>();
        string json = File.ReadAllText(userFilePath);
        return JsonUtility.FromJson<UserList>(json).users;
    }

    /// <summary>
    /// Retorna el usuario actual.
    /// </summary>
    public string GetCurrentUser()
    {
        return currentUser;
    }

    /// <summary>
    /// Guarda el nombre del usuario en el archivo JSON si aún no existe.
    /// </summary>
    private void SaveUser(string name)
    {
        List<string> existing = GetUsers();
        if (!existing.Contains(name))
        {
            existing.Add(name);
            string json = JsonUtility.ToJson(new UserList { users = existing }, true);
            File.WriteAllText(userFilePath, json);
        }
    }

    #endregion

    #region Manejo al Cerrar la Aplicación

    void OnApplicationQuit()
    {
        // Borra el archivo de estado del tutorial al cerrar la aplicación.
        if (File.Exists(tutorialSeenPath))
        {
            File.Delete(tutorialSeenPath);
        }
    }

    #endregion
}
