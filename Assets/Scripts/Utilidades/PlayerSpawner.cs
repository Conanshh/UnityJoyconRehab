using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase responsable de instanciar al jugador basado en el personaje previamente seleccionado 
/// y de inicializar las referencias necesarias según el tipo de nivel (Carretera o Cueva).
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// Tipo de nivel en el cual se instanciará el jugador. Por defecto es Carretera.
    /// </summary>
    public NivelTipo nivelTipo = NivelTipo.Carretera;

    /// <summary>
    /// Punto de spawn en donde se instanciará el jugador.
    /// </summary>
    public Transform spawnPoint;

    [Header("Comunes")]
    public GameObject pausePanel;
    public GameObject backText;

    [Header("Carretera")]
    public GameObject tilemap;
    public GameObject redOverlay;
    public ObstacleSpawner obstacleSpawner;
    public RewardSpawner rewardSpawner;
    public SpawnManager spawnManager;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI messageText;

    [Header("Cueva")]
    public TextMeshProUGUI gemText;
    public GemsSpawner gemsSpawner;
    public GameObject feedbackText;
    public CameraShake cameraShake;
    public GameObject redOverlayCave;

    /// <summary>
    /// En Start se recupera el CharacterData seleccionado (o se usa el default)
    /// y se instancia el prefab correspondiente en la posición de spawn. 
    /// Luego se inicializa el script del jugador en función del tipo de nivel y la escena.
    /// </summary>
    void Start()
    {
        // Obtener el personaje seleccionado para el nivel indicado
        var data = CharacterManager.Instance.GetSelectedCharacter(nivelTipo);
        if (data == null)
        {
            Debug.LogWarning("No se seleccionó un personaje, usando el primero por defecto.");
            data = CharacterManager.Instance.GetDefaultCharacter(nivelTipo);
        }

        // Instanciar el prefab del jugador en el punto de spawn
        GameObject player = Instantiate(data.characterPrefab, spawnPoint.position, Quaternion.identity);
        string sceneName = SceneManager.GetActiveScene().name;

        if (nivelTipo == NivelTipo.Carretera)
        {
            // El jugador en Carretera debe implementar IPlayerCarretera
            var playerLogic = player.GetComponent<IPlayerCarretera>();
            if (sceneName == "TutorialAbAd")
            {
                playerLogic?.Init(tilemap, redOverlay, null, null, null, coinText, messageText, pausePanel, backText);
            }
            else
            {
                playerLogic?.Init(tilemap, redOverlay, obstacleSpawner, rewardSpawner, spawnManager, coinText, messageText, pausePanel, backText);
            }
        }
        else if (nivelTipo == NivelTipo.Cueva)
        {
            // En Cueva se espera que el jugador implemente IPlayerCueva
            var playerLogic = player.GetComponent<IPlayerCueva>();
            PlayerCaveMovement pcm = player.GetComponent<PlayerCaveMovement>();
            if (sceneName == "TutorialFE")
            {
                playerLogic?.Init(gemText, null, pausePanel, backText, feedbackText, null, null);
            }
            else
            {
                playerLogic?.Init(gemText, gemsSpawner, pausePanel, backText, feedbackText, cameraShake, redOverlayCave);
                pcm?.InitializeComponents(cameraShake, redOverlayCave, feedbackText);
            }
        }
    }
}
