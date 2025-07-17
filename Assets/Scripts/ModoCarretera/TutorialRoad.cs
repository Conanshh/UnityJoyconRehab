using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Gestiona la secuencia del tutorial del nivel Road, mostrando mensajes y activando acciones (movimiento, spawn de obstáculos, monedas, etc.)
/// para guiar al jugador en las mecánicas básicas.
/// </summary>
public class TutorialRoad : MonoBehaviour
{
    // Referencia al jugador en la carretera.
    private PlayerRoad player;

    // Referencia al script de movimiento del jugador.
    private PlayerRoadMovement playerMovement;

    // Texto para mostrar mensajes del tutorial.
    public TextMeshProUGUI tutorialText;

    // Texto de fondo que se muestra junto al tutorial.
    public GameObject backText;

    // Temporizador del nivel utilizado para pausar el juego durante el tutorial.
    public Road road;

    // Prefab de obstáculo utilizado en el tutorial.
    public GameObject obstaclePrefab;

    // Prefab de moneda utilizado en el tutorial.
    public GameObject coinPrefab;

    // Panel que se muestra al finalizar el tutorial.
    public GameObject endTutorialPanel;

    // Bandera para manejar la colisión con un obstáculo.
    private bool playerHitObstacle = false;

    void Start()
    {
        StartCoroutine(WaitForPlayerThenStartTutorial());
    }

    /// <summary>
    /// Espera a que el jugador (PlayerRoad) se encuentre en escena y luego inicia el tutorial.
    /// </summary>
    IEnumerator WaitForPlayerThenStartTutorial()
    {
        yield return new WaitUntil(() => FindFirstObjectByType<PlayerRoad>() != null);

        player = FindFirstObjectByType<PlayerRoad>();
        playerMovement = player.GetComponent<PlayerRoadMovement>();

        StartCoroutine(StartTutorial());
    }

    /// <summary>
    /// Ejecuta la secuencia del tutorial mostrando mensajes, controlando movimientos y 
    /// mostrando ejemplos de obstáculos y monedas.
    /// </summary>
    IEnumerator StartTutorial()
    {
        // **Detener el nivel antes de comenzar**
        road.StopTimer();
        player.enabled = false; // Desactivar el script del jugador
        playerMovement.enabled = false; // Desactivar movimiento del jugador

        // **Mensajes iniciales**
        yield return ShowMessage("Hola, en este juego debes esquivar los obstáculos\n y recoger la mayor cantidad de monedas posible.");
        yield return ShowMessage("Te moverás inclinando tu muñeca con el Joy-Con.");
        bool isLeftJoycon = playerMovement.isLeftJoycon;

        // Movimiento hacia la izquierda
        string tipoIzq = isLeftJoycon ? "Abducción" : "Aducción";
        yield return ShowPersistentMessage($"Gira tu muñeca hacia la izquierda para moverte.\n(Esta acción corresponde a una {tipoIzq})");
        player.enabled = true; // Activar el script del jugador
        playerMovement.enabled = true; // Activar movimiento del jugador
        yield return WaitForMovement("izquierda");

        // Volver al centro (desde la izquierda)
        string tipoDer = isLeftJoycon ? "Aducción" : "Abducción";
        yield return ShowPersistentMessage($"Ahora, gira levemente tu muñeca hacia la derecha para volver al centro.\n(Esta acción corresponde a una {tipoDer})");
        yield return WaitForMovement("derecha", true);

        // Movimiento hacia la derecha
        yield return ShowPersistentMessage($"Ahora, gira tu muñeca hacia la derecha para moverte.\n(Esta acción corresponde a una {tipoDer})");
        yield return WaitForMovement("derecha");

        // Volver al centro (desde la derecha)
        yield return ShowPersistentMessage($"Ahora, gira levemente tu muñeca hacia la izquierda para volver al centro.\n(Esta acción corresponde a una {tipoIzq})");
        yield return WaitForMovement("izquierda", true);


        /*
        // **Movimiento inicial (dependiendo del Joy-Con)**
        string moveDirection = playerMovement.isLeftJoycon ? "izquierda" : "derecha";
        string movimiento = playerMovement.isLeftJoycon ? "abducción" : "aducción";
        yield return ShowPersistentMessage($"Gira tu muñeca hacia la {moveDirection} para moverte.\n(Esta acción corresponde a una {movimiento})");
        //yield return ShowPersistentMessage($"Primero, Gira tu muñeca hacia la {moveDirection} para moverte.");
        player.enabled = true; // Activar el script del jugador
        playerMovement.enabled = true; // Activar movimiento del jugador
        yield return WaitForMovement(moveDirection);

        // **Volver al centro**
        string returnDirection = playerMovement.isLeftJoycon ? "derecha" : "izquierda";
        string movimientoRetorno = playerMovement.isLeftJoycon ? "aducción" : "abducción";
        yield return ShowPersistentMessage($"Ahora, Gira levemente tu muñeca hacia la {returnDirection} para volver al centro.\n(Esta acción corresponde a una {movimientoRetorno})");
        yield return WaitForMovement(returnDirection, true);

        // **Movimiento al otro lado**
        yield return ShowPersistentMessage($"Ahora, Gira tu muñeca hacia la {returnDirection} para moverte.\n(Esta acción corresponde a una {movimientoRetorno})");
        yield return WaitForMovement(returnDirection);

        // **Volver al centro nuevamente**
        yield return ShowPersistentMessage($"Ahora, Gira levemente tu muñeca hacia la {moveDirection} para volver al centro.\n(Esta acción corresponde a una {movimiento})");
        yield return WaitForMovement(moveDirection, true);
        */

        // **Tutorial de obstáculos**
        yield return ShowObstacleTutorial();

        road.ResumeTimer(); // Reanudar el temporizador del fondo infinito
        // **Tutorial de monedas**
        yield return ShowCoinTutorial();

        // **Información sobre la barra de tiempo**
        yield return ShowMessage("La barra verde a la derecha disminuirá con el tiempo hasta terminar el nivel.");

        // **Final del tutorial**
        yield return ShowMessage("¡Bien hecho! Ahora recoge monedas y esquiva los obstáculos. ¡Buena suerte!");

        // **Finalizar tutorial**
        Time.timeScale = 0; // Congela toda la escena
        endTutorialPanel.SetActive(true); // Mostrar panel de finalización
    }

    /// <summary>
    /// Muestra la secuencia del obstáculo en el tutorial, spawneando un obstáculo y esperando 
    /// que el jugador lo esquive.
    /// </summary>
    IEnumerator ShowObstacleTutorial()
    {
        Vector3 spawnPosition = new Vector3(playerMovement.lanePositions[1], 0.79f, 0); // Carril central
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        AudioManager.Instance.PlaySpawnObstacle();
        obstacle.GetComponent<ObstacleTutorial>().fallSpeed = 0.2f;

        yield return ShowMessage("Aparecerán obstáculos como automóviles en los tres carriles.\nEsquívalos girando la muñeca.");

        yield return WaitForAnySideMovement(); // Espera a que el jugador se mueva lateralmente

        if (playerHitObstacle)
        {
            playerHitObstacle = false; // Reiniciar la variable
        }
        else
        {
            yield return ShowPersistentMessage("Espera a que pase el auto y vuelve al centro para continuar.");
            yield return WaitForMovement(playerMovement.isLeftJoycon ? "izquierda" : "derecha", true);

            Destroy(obstacle); // Elimina el obstáculo
        }
    }

    /// <summary>
    /// Muestra la secuencia para el tutorial de monedas, spawneando una moneda y explicando la mecánica.
    /// </summary>
    IEnumerator ShowCoinTutorial()
    {
        Vector3 spawnPosition = new Vector3(playerMovement.lanePositions[2], 0.79f, 0); // Carril derecho
        GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        coin.GetComponent<RewardTutorial>().fallSpeed = 0.3f;

        yield return ShowMessage("Recoge monedas chocando con ellas.");
        yield return ShowPersistentMessage("Obtén la mayor cantidad posible.");

        // **Esperar a que el jugador toque la moneda**
        yield return ShowPersistentMessage("Toma la mondea y vuelve al centro para continuar.");
        yield return new WaitUntil(() => coin == null); // Espera a que la moneda sea recolectada (destruida)

        // **Esperar a que el jugador vuelva al centro**
        yield return WaitForMovement("izquierda", true);

        // **Advertencia sobre los obstáculos (en negrita)**
        yield return ShowMessage("<b>¡Cuidado!</b> Si chocas con un obstáculo, perderás las monedas recolectadas.");
    }

    /// <summary>
    /// Registra la colisión en el tutorial para indicar que el jugador fue impactado por un obstáculo.
    /// </summary>
    public void HandleCollisionTutorial(string type)
    {
        if (type == "obstacle")
        {
            playerHitObstacle = true;
        }
    }

    /// <summary>
    /// Muestra un mensaje de forma temporal durante el tutorial.
    /// </summary>
    IEnumerator ShowMessage(string message)
    {
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = message;
        tutorialText.gameObject.SetActive(true);
        backText.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        tutorialText.gameObject.SetActive(false);
        backText.SetActive(false);
    }

    /// <summary>
    /// Muestra un mensaje persistente hasta que otra acción decida ocultarlo.
    /// </summary>
    IEnumerator ShowPersistentMessage(string message)
    {
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = message;
        tutorialText.gameObject.SetActive(true);
        backText.SetActive(true); // Activa el texto de fondo
        yield return null; // No se oculta hasta que termine la acción
    }

    /// <summary>
    /// Espera hasta que el jugador se mueva a un carril lateral o se acabe un tiempo determinado.
    /// </summary>
    IEnumerator WaitForAnySideMovement()
    {
        float timer = 0f;
        float timeout = 5f;

        while (true)
        {
            if (playerMovement.currentLane == 0 || playerMovement.currentLane == 2)
            {
                yield break;
            }

            // Si pasaron 5 segundos y hubo colisión, simula el movimiento
            if (playerHitObstacle && timer >= timeout)
            {
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Espera hasta que el jugador se mueva en la dirección indicada o regrese al centro.
    /// </summary>
    IEnumerator WaitForMovement(string direction, bool returnToCenter = false)
    {
        yield return new WaitUntil(() =>
        {
            if (returnToCenter)
            {
                return playerMovement.currentLane == 1; // Verificar si está en el centro
            }
            else
            {
                return (direction == "izquierda" && playerMovement.currentLane == 0) ||
                       (direction == "derecha" && playerMovement.currentLane == 2);
            }
        });
        tutorialText.gameObject.SetActive(false);
    }
}
