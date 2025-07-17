using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Clase responsable de cargar din�micamente los botones de selecci�n de niveles en la interfaz.
/// Utiliza una lista de niveles para instanciar botones que, al ser presionados, cargan la escena correspondiente.
/// </summary>
public class DynamicLevelLoader : MonoBehaviour
{
    /// <summary>
    /// Lista que contiene la informaci�n de cada nivel disponible.
    /// </summary>
    public LevelList levelList;

    /// <summary>
    /// Prefab del bot�n que se usar� para representar cada nivel.
    /// </summary>
    public GameObject buttonPrefab;

    /// <summary>
    /// Contenedor padre en el que se instanciar�n los botones.
    /// </summary>
    public Transform buttonContainer;

    /// <summary>
    /// Inicializa la interfaz de selecci�n de niveles, generando un bot�n para cada nivel disponible.
    /// </summary>
    void Start()
    {
        foreach (var level in levelList.levels)
        {
            // Se instancia un bot�n a partir del prefab y se coloca en el contenedor.
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);

            // Se actualiza el texto del bot�n para mostrar el nombre del nivel.
            btn.GetComponentInChildren<TMP_Text>().text = level.levelName;

            // Se asigna el nombre de la escena para cargar posteriormente.
            // Se almacena en una variable local para evitar problemas con cierres de variable.
            string sceneToLoad = level.sceneName; // Necesario para evitar error de cierre sobre variable

            // Se configura el listener del bot�n para cargar la escena correspondiente al ser presionado.
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayClickSound();
                SceneManager.LoadScene(sceneToLoad);
            });
        }
    }

    /// <summary>
    /// M�todo para regresar al men� principal.
    /// Reproduce un sonido de clic, restablece la escala del tiempo y carga la escena del men� principal.
    /// </summary>
    public void GoToMainMenu() // Para regresar al men�a principal
    {
        AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1; // Se asegura que la escala del tiempo est� en normal.
        SceneManager.LoadScene("MainMenu"); // Cargar la escena del men� principal
    }
}
