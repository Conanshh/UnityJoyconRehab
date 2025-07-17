using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona el menú de ajustes para el modo Cueva, permitiendo configurar el umbral de movimiento,
/// el tiempo de juego y la selección del Joy-Con (izquierdo o derecho). Además, actualiza la interfaz (UI)
/// y aplica las configuraciones utilizando GameData.
/// </summary>
public class SettingsMenuCave : MonoBehaviour
{
    // Joy-Con
    private List<Joycon> joycons;
    private bool isLeftJoycon; // Valor por defecto

    [Header("Indicadores de Selección de Joy-Con")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;
    public TextMeshProUGUI leftJoyconText;
    public TextMeshProUGUI rightJoyconText;

    [Header("Ajustes del Umbral")]
    public TextMeshProUGUI thresholdValueText;
    public Button increaseThresholdButton;
    public Button decreaseThresholdButton;

    [Header("Ajustes del Tiempo de Juego")]
    public TMP_InputField gameTimeInputField;
    public TextMeshProUGUI gameTimeValueText;

    [Header("Botones de Aplicación y Navegación")]
    public Button applyButton;
    public Button backButton;

    [Header("Paneles de Interfaz")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    // Referencias al Player y sus scripts
    private GameObject playerInstance; // Referencia al Player en la escena
    private PlayerTestMovesCave movementScript; // Script de movimiento del modo Cueva
    private Animator playerAnimator; // Animator del jugador
    
    // Valores de configuración
    private int gameTime = 60; // Valor predeterminado en segundos
    private float thresholdFE = 10f; // Umbral de movimiento predeterminado para el modo Cueva
    private float minThreshold = 0f;
    private float maxThreshold = 200f;

    // Datos del juego
    private GameData gameData;

    void Awake()
    {
        // Agregar listeners a los botones de umbral y tiempo de juego
        increaseThresholdButton.onClick.AddListener(IncreaseThreshold);
        decreaseThresholdButton.onClick.AddListener(DecreaseThreshold);

        gameTimeInputField.onEndEdit.AddListener(ValidateGameTime);
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMenuPanel);
    }

    void Start()
    {
        // Cargar datos guardados desde GameData
        gameData = GameData.Load();
        thresholdFE = gameData.thresholdAngleFE;
        isLeftJoycon = gameData.isLeftJoyCon; // Configuración del Joy-Con
        gameTime = gameData.GameTime;

        // Obtener la lista de Joy-Con detectados
        joycons = JoyconManager.Instance.j;
        Debug.Log("Joycons detectados: " + joycons.Count);
        DetectConnectedJoycon(); // En caso de haber solo 1 conectado

        // Actualizar la interfaz con los valores cargados
        UpdateUI();

        // Buscar el Player en la escena en lugar de instanciarlo (se usa PlayerTest2)
        playerInstance = GameObject.FindGameObjectWithTag("PlayerTest2");
        if (playerInstance != null)
        {
            movementScript = playerInstance.GetComponent<PlayerTestMovesCave>();
            playerAnimator = playerInstance.GetComponent<Animator>();

            // Bloquear movimiento y animaciones hasta aplicar las configuraciones
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }
            if (playerAnimator != null)
            {
                playerAnimator.enabled = false;
            }
        }
    }

    #region Detección y Actualización de Selección de Joy-Con

    void Update()
    {
        foreach (var joycon in joycons)
        {
            if (joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                isLeftJoycon = joycon.isLeft;
                UpdateJoyconSelection();
            }
        }
    }

    /// <summary>
    /// Detecta si hay un único Joy-Con conectado y lo asigna.
    /// </summary>
    void DetectConnectedJoycon()
    {
        Debug.Log("Detectando JoyCon conectado...");
        if (joycons.Count == 1)
        {
            isLeftJoycon = joycons[0].isLeft;
        }
    }

    /// <summary>
    /// Actualiza la selección visual del Joy-Con, actualiza GameData y aplica una vibración breve.
    /// </summary>
    void UpdateJoyconSelection()
    {
        // Actualizar la selección en GameData y guardar los cambios
        gameData.isLeftJoyCon = isLeftJoycon;
        gameData.Save();
        // Recargar los datos para obtener los cambios recientes
        gameData = GameData.Load();
        isLeftJoycon = gameData.isLeftJoyCon;

        // Actualizar la interfaz para resaltar la selección del Joy-Con
        if (isLeftJoycon)
        {
            leftJoyconText.fontStyle = FontStyles.Bold;
            rightJoyconText.fontStyle = FontStyles.Normal;
        }
        else
        {
            leftJoyconText.fontStyle = FontStyles.Normal;
            rightJoyconText.fontStyle = FontStyles.Bold;
        }

        // Aplicar vibración en el Joy-Con seleccionado
        foreach (var joycon in joycons)
        {
            if (joycon.isLeft == isLeftJoycon)
            {
                joycon.SetRumble(160, 320, 0.6f, 200); // Vibración breve en el Joy-Con seleccionado
            }
        }
    }

    #endregion

    #region Ajustes de Umbral y Tiempo de Juego

    /// <summary>
    /// Incrementa el umbral de movimiento respetando el valor máximo permitido.
    /// Sube de 1 en 1 hasta 5, luego de 5 en 5.
    /// </summary>
    void IncreaseThreshold()
    {
        AudioManager.Instance.PlayClickSound();
        float stepToUse = thresholdFE < 5f ? 1f : 5f;
        if (thresholdFE + stepToUse <= maxThreshold)
        {
            thresholdFE += stepToUse;
            UpdateThresholdUI();
        }
        else if (thresholdFE < maxThreshold)
        {
            thresholdFE = maxThreshold;
            UpdateThresholdUI();
        }
    }

    /// <summary>
    /// Decrementa el umbral de movimiento respetando el valor mínimo permitido.
    /// Baja de 5 en 5 hasta 5, luego de 1 en 1.
    /// </summary>
    void DecreaseThreshold()
    {
        AudioManager.Instance.PlayClickSound();
        float stepToUse = thresholdFE > 5f ? 5f : 1f;
        if (thresholdFE - stepToUse >= minThreshold)
        {
            thresholdFE -= stepToUse;
            UpdateThresholdUI();
        }
        else if (thresholdFE > minThreshold)
        {
            thresholdFE = minThreshold;
            UpdateThresholdUI();
        }
    }

    /// <summary>
    /// Actualiza en la interfaz el valor actual del umbral.
    /// </summary>
    void UpdateThresholdUI()
    {
        thresholdValueText.text = thresholdFE.ToString("F2");
    }

    /// <summary>
    /// Valida el tiempo de juego ingresado y actualiza la interfaz.
    /// </summary>
    /// <param name="input">Entrada de texto del usuario.</param>
    void ValidateGameTime(string input)
    {
        if (int.TryParse(input, out int result) && result >= 1 && result <= 300)
        {
            gameTime = result;
            gameTimeValueText.text = result.ToString("F0") + "s"; 
            gameTimeInputField.text = result.ToString();
        }
        else
        {
            gameTimeValueText.text = "Valor inválido";
        }
    }

    #endregion

    #region Actualización de la Interfaz (UI)
    /// <summary>
    /// Actualiza la interfaz mostrando el umbral, el tiempo de juego y la selección del Joy-Con.
    /// </summary>
    void UpdateUI()
    {
        UpdateThresholdUI();
        gameTimeValueText.text = gameTime.ToString("F0") + "s";
        gameTimeInputField.text = gameTime.ToString();
        UpdateJoyconSelection();
    }

    #endregion

    #region Aplicación de Configuraciones y Navegación

    /// <summary>
    /// Aplica las configuraciones actuales, guarda los cambios en GameData,
    /// y habilita el movimiento y las animaciones del jugador.
    /// </summary>
    void ApplySettings()
    {
        AudioManager.Instance.PlayClickSound();
        // Guardar los nuevos valores en GameData
        gameData.thresholdAngleFE = thresholdFE; 
        gameData.GameTime = gameTime;
        gameData.Save();

        // Habilitar el script de movimiento y el Animator del jugador
        if (movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.UpdateJoyconIndex(); // Actualizar índice del Joy-Con del PlayerTest
        }
        if (playerAnimator != null)
        {
            playerAnimator.enabled = true;
        }

    }

    /// <summary>
    /// Regresa al panel principal del menú, deshabilitando las funcionalidades del jugador.
    /// </summary>
    void BackToMenuPanel()
    {
        AudioManager.Instance.PlayClickSound();
        if (movementScript != null)
        {
            movementScript.ResetToCenter();
            movementScript.enabled = false;
        }
        if (playerAnimator != null)
        {
            playerAnimator.enabled = false;
        }
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    #endregion
}
