using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Clase responsable de cargar dinámicamente los botones de selección de niveles en la interfaz.
/// Utiliza una lista de niveles para instanciar botones que, al ser presionados, cargan la escena correspondiente.
/// </summary>
public class DynamicLevelLoader : MonoBehaviour
{
    /// <summary>
    /// Lista que contiene la información de cada nivel disponible.
    /// </summary>
    public LevelList levelList;

    /// <summary>
    /// Prefab del botón que se usará para representar cada nivel.
    /// </summary>
    public GameObject buttonPrefab;

    /// <summary>
    /// Contenedor padre en el que se instanciarán los botones.
    /// </summary>
    public Transform buttonContainer;

    /// <summary>
    /// Inicializa la interfaz de selección de niveles, generando un botón para cada nivel disponible.
    /// </summary>
    void Start()
    {
        foreach (var level in levelList.levels)
        {
            // Se instancia un botón a partir del prefab y se coloca en el contenedor.
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);

            // Se actualiza el texto del botón para mostrar el nombre del nivel.
            btn.GetComponentInChildren<TMP_Text>().text = level.levelName;

            // Se asigna el nombre de la escena para cargar posteriormente.
            // Se almacena en una variable local para evitar problemas con cierres de variable.
            string sceneToLoad = level.sceneName; // Necesario para evitar error de cierre sobre variable

            // Se configura el listener del botón para cargar la escena correspondiente al ser presionado.
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayClickSound();
                SceneManager.LoadScene(sceneToLoad);
            });
        }
    }

    /// <summary>
    /// Método para regresar al menú principal.
    /// Reproduce un sonido de clic, restablece la escala del tiempo y carga la escena del menú principal.
    /// </summary>
    public void GoToMainMenu() // Para regresar al menúa principal
    {
        AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1; // Se asegura que la escala del tiempo esté en normal.
        SceneManager.LoadScene("MainMenu"); // Cargar la escena del menú principal
    }
}
