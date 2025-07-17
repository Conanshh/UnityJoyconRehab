using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona el menú de ajustes del juego, permitiendo configurar el umbral de movimiento, el tiempo de juego 
/// y la selección del Joy-Con (izquierdo o derecho). Además, actualiza la interfaz (UI) y aplica las configuraciones usando GameData.
/// </summary>
public class SettingsMenuRoad : MonoBehaviour
{
    private List<Joycon> joycons;              // Lista de Joy-Con detectados
    private bool isLeftJoycon;                 // Determina si se usará el Joy-Con izquierdo

    [Header("Colores Indicadores")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    [Header("Textos de Selección de Joy-Con")]
    public TextMeshProUGUI leftJoyconText;
    public TextMeshProUGUI rightJoyconText;

    [Header("Ajustes del Umbral")]
    public TextMeshProUGUI thresholdValueText;
    public Button increaseThresholdButton;
    public Button decreaseThresholdButton;

    [Header("Ajustes del Tiempo de Juego")]
    public TMP_InputField gameTimeInputField;
    public TextMeshProUGUI gameTimeValueText;

    [Header("Botones de Aplicación y Regreso")]
    public Button applyButton;
    public Button backButton;

    [Header("Referencias a Paneles")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Tutorial UI")]
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;

    // Variables de configuración
    private int gameTime = 60;                // Tiempo de juego predeterminado (en segundos)
    private float threshold = 10f;            // Umbral predeterminado para el movimiento
    private float minThreshold = 0f;          // Umbral mínimo permitido
    private float maxThreshold = 200f;        // Umbral máximo permitido

    // Referencias al Player y su script de movimiento
    private GameObject playerInstance;        // Instancia del jugador (PlayerTest en escena)
    private PlayerTestMovesRoad movementScript; // Referencia al script de movimiento del modo Carretera
    private Animator playerAnimator;          // Animator del jugador

    private GameData gameData; // Instancia para acceder y guardar los datos del juego


    void Awake()
    {        
        // Agregar listeners a los botones de umbral y a los de tiempo de juego
        increaseThresholdButton.onClick.AddListener(IncreaseThreshold);
        decreaseThresholdButton.onClick.AddListener(DecreaseThreshold);

        gameTimeInputField.onEndEdit.AddListener(ValidateGameTime);
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMenuPanel);
    }

    void Start()
    {
        // Cargar los datos guardados desde GameData
        gameData = GameData.Load();
        threshold = gameData.thresholdAngleAbAd;
        gameTime = gameData.GameTime;
        isLeftJoycon = gameData.isLeftJoyCon;

        // Detectar Joy-Con conectados
        joycons = JoyconManager.Instance.j;
        DetectConnectedJoycon(); // En caso de haber solo 1 conectado

        // Actualizar la UI con los valores cargados
        UpdateUI();

        // Buscar el Player en la escena para deshabilitar temporalmente movimiento y animaciones
        playerInstance = GameObject.FindGameObjectWithTag("PlayerTest");
        if (playerInstance != null)
        {
            movementScript = playerInstance.GetComponent<PlayerTestMovesRoad>();
            playerAnimator = playerInstance.GetComponent<Animator>();

            if (movementScript != null)
            {
                movementScript.enabled = false; // Bloquear movimiento hasta aplicar cambios
            }
            if (playerAnimator != null)
            {
                playerAnimator.enabled = false; // Pausar animaciones hasta aplicar cambios
            }
        }
    }

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
    /// Detecta si hay un solo Joy-Con conectado y lo asigna.
    /// </summary>
    void DetectConnectedJoycon()
    {
        if (joycons.Count == 1)
        {
            isLeftJoycon = joycons[0].isLeft;
        }
    }

    /// <summary>
    /// Incrementa el valor del umbral en función del paso definido, hasta el máximo permitido.
    /// </summary>
    void IncreaseThreshold()
    {
        AudioManager.Instance.PlayClickSound();
        float stepToUse = threshold < 5f ? 1f : 5f;
        if (threshold + stepToUse <= maxThreshold)
        {
            threshold += stepToUse;
            UpdateThresholdUI();
        }
        else if (threshold < maxThreshold)
        {
            threshold = maxThreshold;
            UpdateThresholdUI();
        }
    }

    /// <summary>
    /// Decrementa el valor del umbral en función del paso definido, hasta el mínimo permitido.
    /// </summary>
    void DecreaseThreshold()
    {
        AudioManager.Instance.PlayClickSound();
        float stepToUse = threshold > 5f ? 5f : 1f;
        if (threshold - stepToUse >= minThreshold)
        {
            threshold -= stepToUse;
            UpdateThresholdUI();
        }
        else if (threshold > minThreshold)
        {
            threshold = minThreshold;
            UpdateThresholdUI();
        }
    }

    /// <summary>
    /// Actualiza la interfaz para mostrar el valor actual del umbral.
    /// </summary>
    void UpdateThresholdUI()
    {
        thresholdValueText.text = threshold.ToString("F2");
    }

    /// <summary>
    /// Valida y actualiza el tiempo de juego ingresado, asegurando que se encuentre en el rango permitido.
    /// </summary>
    /// <param name="input">Texto ingresado por el usuario.</param>
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

    /// <summary>
    /// Actualiza la selección del Joy-Con, actualizando GameData y la interfaz.
    /// </summary>
    void UpdateJoyconSelection()
    {
        // Actualiza la selección en GameData y guarda los cambios
        gameData.isLeftJoyCon = isLeftJoycon;
        gameData.Save();

        // Recargar la configuración para reflejar cambios recientes
        gameData = GameData.Load();
        isLeftJoycon = gameData.isLeftJoyCon;

        // Actualizar la interfaz para resaltar el Joy-Con seleccionado
        if (isLeftJoycon)
        {
            leftJoyconText.fontStyle = FontStyles.Bold;
            rightJoyconText.fontStyle = FontStyles.Normal;
            leftText.text = "Abducción";
            rightText.text = "Aducción";
        }
        else
        {
            leftJoyconText.fontStyle = FontStyles.Normal;
            rightJoyconText.fontStyle = FontStyles.Bold;
            leftText.text = "Aducción";
            rightText.text = "Abducción";
        }

        // Aplicar una vibración breve en el Joy-Con seleccionado
        foreach (var joycon in joycons)
        {
            if (joycon.isLeft == isLeftJoycon)
            {
                joycon.SetRumble(160, 320, 0.6f, 200); // Vibración breve en el Joy-Con seleccionado
            }
        }
    }

    /// <summary>
    /// Aplica las configuraciones actuales, guardándolas en GameData
    /// y habilitando el movimiento y las animaciones del jugador.
    /// </summary>
    void ApplySettings()
    {
        AudioManager.Instance.PlayClickSound();

        // Usar GameData para guardar los nuevos valores
        gameData.thresholdAngleAbAd = threshold;
        gameData.GameTime = gameTime; 
        gameData.Save();

        // Permitir el movimiento y las animaciones del jugador
        if (movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.UpdateJoyconIndex();
        }
        if (playerAnimator != null)
        {
            playerAnimator.enabled = true;
        }

    }

    /// <summary>
    /// Regresa al panel principal del menú, deshabilitando el script de movimiento y animador del jugador.
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

    /// <summary>
    /// Actualiza la interfaz (UI) mostrando los valores actuales del umbral y del tiempo de juego, 
    /// y actualiza la selección visual del Joy-Con.
    /// </summary>
    void UpdateUI()
    {
        UpdateThresholdUI();
        gameTimeValueText.text = gameTime.ToString("F0") + "s";
        gameTimeInputField.text = gameTime.ToString();
        UpdateJoyconSelection();
    }
}
