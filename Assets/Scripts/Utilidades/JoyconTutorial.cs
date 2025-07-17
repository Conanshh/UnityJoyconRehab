using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona y orquesta el tutorial del Joy-Con, proporcionando instrucciones al usuario mediante mensajes
/// y transiciones de posición y rotación del modelo 3D. También permite saltarse el tutorial.
/// </summary>
public class JoyconTutorial : MonoBehaviour
{
    [Header("Referencias")]
    public Transform joyconModel;         // Modelo 3D del Joy-Con
    public TextMeshProUGUI tutorialText;    // Texto de instrucciones del tutorial
    public GameObject skipButton;           // Botón para saltar el tutorial

    [Header("Configuración del Tutorial")]
    public float rotationDuration = 1.5f;   // Duración de cada transición de rotación

    private Coroutine tutorialCoroutine;

    void Start()
    {
        tutorialCoroutine = StartCoroutine(RunTutorial());
    }

    /// <summary>
    /// Ejecuta la secuencia del tutorial del Joy-Con, presentando instrucciones y realizando transiciones.
    /// Al finalizar, marca el tutorial como visto y carga la escena "Levels".
    /// </summary>
    IEnumerator RunTutorial()
    {
        // Configurar posición y rotación inicial
        joyconModel.localPosition = new Vector3(0.272f, -0.245000005f, -0.0209999997f);
        joyconModel.localRotation = new Quaternion(-0.491817713f, -0.494460374f, 0.504958928f, 0.508567452f);
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = "Sostén el Joy-Con de forma vertical, con los botones apuntando hacia ti.";
        yield return new WaitForSeconds(3f);

        // Primera transición: joy-con acostadito, con los botones mirándote.
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = "Ahora rota el Joy-Con para que quede acostadito, con los botones mirandote.";
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = "Y los botones SL y SR apuntnado al cielo.";
        yield return RotateJoycon(
            new Vector3(0.272f, -0.248294905f, -0.0209208503f),
            new Quaternion(-0.702195406f, -0.00371993496f, -0.000669700501f, 0.711974263f)
        );
        yield return new WaitForSeconds(2f);

        // Segunda transición: joy-con con botones SL y SR apuntando hacia la pantalla.
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = "Finalmente, rota el JoyCon de manera que los botones SL y SR apunten hacia la pantalla.";
        yield return RotateJoycon(
            new Vector3(0.272f, -0.228265628f, -0.014176812f),
            new Quaternion(-0.0404427126f, -0.00301526557f, -0.00262059481f, 0.99917388f)
        );
        yield return new WaitForSeconds(3f);

        // Mensaje final e inicio del juego
        AudioManager.Instance.PlayTutorialSound();
        tutorialText.text = "¡Perfecto! Estás listo para comenzar. Recuerda tomarlo de manera que tus dedos de la mano cubran los botones.";
        UserManager.Instance.MarkTutorialSeen(UserManager.Instance.GetCurrentUser());
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Levels");
    }

    /// <summary>
    /// Realiza una transición suave del modelo del Joy-Con hacia la posición y rotación objetivo.
    /// </summary>
    /// <param name="targetPosition">Posición local objetivo.</param>
    /// <param name="targetRotation">Rotación local objetivo.</param>
    IEnumerator RotateJoycon(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPos = joyconModel.localPosition;
        Quaternion startRot = joyconModel.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            joyconModel.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            joyconModel.localRotation = Quaternion.Slerp(startRot, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurar que se aplican los valores finales
        joyconModel.localPosition = targetPosition;
        joyconModel.localRotation = targetRotation;
    }

    /// <summary>
    /// Permite al usuario saltarse el tutorial.
    /// Detiene la secuencia actual, marca el tutorial como visto y carga la escena "Levels".
    /// </summary>
    public void SkipTutorial()
    {
        if (tutorialCoroutine != null)
            StopCoroutine(tutorialCoroutine);
        UserManager.Instance.MarkTutorialSeen(UserManager.Instance.GetCurrentUser());
        tutorialText.text = "Tutorial omitido.";
        SceneManager.LoadScene("Levels");
    }
}
