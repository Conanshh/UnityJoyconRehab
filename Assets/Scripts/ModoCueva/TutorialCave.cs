using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Controla el tutorial de la cueva, guiando al jugador mediante mensajes y spawns específicos de gemas.
/// Se detiene el temporizador del fondo, se muestran mensajes, se spawnea la gema en posiciones fijas
/// y se espera a que el jugador se ubique en dichos puntos para continuar.
/// </summary>
public class TutorialCave : MonoBehaviour
{
    [Header("Referencias del Tutorial")]
    public PlayerCave player;         // Script del jugador
    public PlayerCaveMovement PlayerMovementScript; // Script de movimiento del jugador
    public TextMeshProUGUI tutorialText; // Texto a mostrar en el tutorial
    public GameObject TextBack;       // Texto de fondo para el tutorial
    public Cave cave;                 // Se utiliza para pausar/reanudar el temporizador del fondo
    public GameObject gemPrefab;      // Prefab de la gema para el tutorial
    public GameObject endTutorialPanel; // Panel que se muestra al finalizar el tutorial

    [Header("Configuración")]
    public float spawntime = 4.0f;     // Tiempo de espera para mostrar los mensajes

    private GameObject currentGem;    // Referencia a la gema spawneada

    /// <summary>
    /// Inicia el proceso del tutorial esperando la existencia del jugador y luego comenzando la secuencia.
    /// </summary>
    void Start()
    {
        StartCoroutine(WaitForPlayerThenStartTutorial());
    }

    #region Tutorial Sequence
    /// <summary>
    /// Espera a que el objeto PlayerCave exista en la escena y luego inicia el tutorial.
    /// </summary>
    /// <returns>IEnumerator para la corrutina.</returns>
    IEnumerator WaitForPlayerThenStartTutorial()
    {
        // Espera a que exista un objeto con el script PlayerCave
        yield return new WaitUntil(() => FindFirstObjectByType<PlayerCave>() != null);

        player = FindFirstObjectByType<PlayerCave>();
        PlayerMovementScript = player.GetComponent<PlayerCaveMovement>();

        // Se desactiva el movimiento del jugador para forzar el flujo del tutorial
        player.enabled = false;
        PlayerMovementScript.enabled = false;

        StartCoroutine(StartTutorial());
    }


    /// <summary>
    /// Orquesta la secuencia del tutorial:
    /// - Detiene el temporizador.
    /// - Muestra mensajes explicativos.
    /// - Spawnea una gema en la posición superior, espera la acción del jugador.
    /// - Spawnea una gema en la posición inferior, espera la acción y finaliza el tutorial.
    /// </summary>
    /// <returns>IEnumerator para la corrutina.</returns>
    IEnumerator StartTutorial()
    {
        // Se detiene el temporizador del nivel.
        cave.StopTimer();
        
        yield return ShowMessage("Hola, en este juego debes atrapar todas las gemas posibles.");
        yield return ShowMessage("Estas saldrán arriba de la cueva o abajo de esta.");

        // Spawnea la primera gema en la posición superior (fija en 0.19f)
        Vector3 topGemPosition = new Vector3(player.transform.position.x, 0.19f, 0f);
        currentGem = Instantiate(gemPrefab, topGemPosition, Quaternion.identity);

        yield return ShowPersistentMessage("Para atrapar esta gema, haz una <b>Extensión</b> para subir por ella.");
        // Activa el jugador para que realice la acción
        player.enabled = true;
        PlayerMovementScript.enabled = true;
        yield return WaitUntilPosition(0.19f);
        // Se pausa nuevamente para el siguiente paso
        player.enabled = false;
        PlayerMovementScript.enabled = false;
        yield return ShowMessage("¡Bien hecho!");

        // Spawnea la segunda gema en la posición inferior (fija en -0.52f)
        Vector3 bottomGemPosition = new Vector3(player.transform.position.x, -0.52f, 0f);
        currentGem = Instantiate(gemPrefab, bottomGemPosition, Quaternion.identity);

        yield return ShowPersistentMessage("Ahora, para atrapar esta nueva gema, haz una <b>Flexión</b> para bajar por ella.");
        player.enabled = true;
        PlayerMovementScript.enabled = true;    
        yield return WaitUntilPosition(-0.52f);
        yield return ShowMessage("¡Bien hecho!");

        // Mensajes finales del tutorial
        yield return ShowMessage("El contador a la izquierda te dirá cuántas gemas llevas.");
        cave.ResumeTimer();  // Se reanuda el temporizador del fondo
        yield return ShowMessage("Y recuerda continuar hasta que la barra de la derecha se acabe.");
        yield return ShowMessage("Muy bien, ¡a jugar!");

        // Finaliza el tutorial pausando el juego y mostrando el panel correspondiente.
        Time.timeScale = 0f;
        endTutorialPanel.SetActive(true);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Muestra un mensaje de forma temporal utilizando el tutorialText y el fondo asociado.
    /// </summary>
    /// <param name="message">Mensaje a mostrar.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
    IEnumerator ShowMessage(string message)
    {
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = message;
        TextBack.SetActive(true);
        yield return new WaitForSeconds(spawntime);
        TextBack.SetActive(false);
    }


    /// <summary>
    /// Muestra un mensaje de forma persistente (sin espera para ocultarlo).
    /// </summary>
    /// <param name="message">Mensaje a mostrar.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
    IEnumerator ShowPersistentMessage(string message)
    {
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = message;
        TextBack.SetActive(true);
        yield return null;
    }

    /// <summary>
    /// Espera hasta que el jugador alcance la posición Y deseada.
    /// Se utiliza para sincronizar la acción del jugador con el tutorial.
    /// </summary>
    /// <param name="yTarget">Posición Y objetivo.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
    IEnumerator WaitUntilPosition(float yTarget)
    {
        yield return new WaitUntil(() =>
        {
            return Mathf.Abs(player.transform.position.y - yTarget) < 0.05f;
        });
        TextBack.SetActive(false);
    }

    #endregion

}
