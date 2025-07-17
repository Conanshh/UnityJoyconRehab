using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GeneralSettingsPanel : MonoBehaviour
{
    [Header("Botones")]
    public Button applyButton;
    public Button backButton;

    [Header("Paneles")]
    public GameObject panelajustes;
    public GameObject mainMenuPanel;

    [Header("Controles de Volumen")]
    public Button musicIncreaseButton;
    public Button musicDecreaseButton;
    public Button sfxIncreaseButton;
    public Button sfxDecreaseButton;
    public TMP_Text musicVolumeText;
    public TMP_Text sfxVolumeText;

    [Header("Ajustes de Pantalla")]
    public Button fullscreenButton;
    public TMP_Text fullscreenButtonText;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown aspectRatioDropdown;

    #region Ajustes de Volumen

    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private const float volumeStep = 0.05f;

    #endregion

    private Resolution[] resolutions;
    private int currentResolutionIndex;

    void Start()
    {
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMainMenu);

        musicIncreaseButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickButtonVolumeSound();
            ChangeMusicVolume(volumeStep);
        });
        musicDecreaseButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickButtonVolumeSound();
            ChangeMusicVolume(-volumeStep);
        });
        sfxIncreaseButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickButtonVolumeSound();
            ChangeSFXVolume(volumeStep);
        });
        sfxDecreaseButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickButtonVolumeSound();
            ChangeSFXVolume(-volumeStep);
        });

        // Listener para el botón de pantalla completa
        fullscreenButton.onClick.AddListener(ToggleFullscreen);
        // Por defecto, pantalla completa activada
        if (!PlayerPrefs.HasKey("IsFullscreen"))
            Screen.fullScreen = true;

        // Agregar sonido de clic al cambiar el valor de los dropdowns
        aspectRatioDropdown.onValueChanged.AddListener(delegate {
            AudioManager.Instance.PlayClickSound();
        });
        resolutionDropdown.onValueChanged.AddListener(delegate {
            AudioManager.Instance.PlayClickSound();
        });

        // Cargar ajustes de volumen guardados
        musicVolume = PlayerPrefs.GetFloat("GeneralMusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("GeneralSFXVolume", 1f);
        UpdateVolumeTexts();
        AudioManager.Instance.SetGeneralMusicVolume(musicVolume);
        AudioManager.Instance.SetGeneralSFXVolume(sfxVolume);

        // Inicializar resoluciones y opciones de relación de aspecto
        InitResolutions();
        InitAspectRatioSettings();
        UpdateFullscreenButtonText();

        LoadSettings();
    }

    /// <summary>
    /// Inicializa el dropdown de resoluciones con las resoluciones disponibles que coinciden con una relación de aspecto 16:9.
    /// </summary>
    void InitResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            float aspectRatio = (float)resolutions[i].width / (float)resolutions[i].height;
            if (Mathf.Abs(aspectRatio - (16f / 9f)) < 0.01f)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Inicializa el dropdown de relación de aspecto con opciones predefinidas.
    /// </summary>
    void InitAspectRatioSettings()
    {
        aspectRatioDropdown.ClearOptions();
        List<string> options = new List<string> { "16:9", "4:3", "21:9" };

        aspectRatioDropdown.AddOptions(options);

        float currentAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(currentAspect - (16f / 9f)) < 0.01f)
            aspectRatioDropdown.value = 0;
        else if (Mathf.Abs(currentAspect - (4f / 3f)) < 0.01f)
            aspectRatioDropdown.value = 1;
        else if (Mathf.Abs(currentAspect - (21f / 9f)) < 0.01f)
            aspectRatioDropdown.value = 2;
        else
            aspectRatioDropdown.value = 0;

        aspectRatioDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Carga los ajustes de pantalla previamente guardados y actualiza la interfaz.
    /// </summary>
    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("AspectRatioSetting"))
        {
            int savedAspectIndex = PlayerPrefs.GetInt("AspectRatioSetting");
            aspectRatioDropdown.value = savedAspectIndex;
            aspectRatioDropdown.RefreshShownValue();
        }

        if (PlayerPrefs.HasKey("ResolutionWidth") && PlayerPrefs.HasKey("ResolutionHeight"))
        {
            int savedWidth = PlayerPrefs.GetInt("ResolutionWidth");
            int savedHeight = PlayerPrefs.GetInt("ResolutionHeight");
            bool savedFullscreen = PlayerPrefs.GetInt("IsFullscreen") == 1;
            Screen.SetResolution(savedWidth, savedHeight, savedFullscreen);

            // Actualizar la selección del dropdown de resoluciones
            for (int i = 0; i < resolutionDropdown.options.Count; i++)
            {
                string[] optionParts = resolutionDropdown.options[i].text.Split('x');
                int optionWidth = int.Parse(optionParts[0].Trim());
                int optionHeight = int.Parse(optionParts[1].Trim());

                if (optionWidth == savedWidth && optionHeight == savedHeight)
                {
                    resolutionDropdown.value = i;
                    resolutionDropdown.RefreshShownValue();
                    break;
                }
            }
            Screen.fullScreen = savedFullscreen;
            UpdateFullscreenButtonText();
        }
    }

    /// <summary>
    /// Aplica los ajustes actuales, los guarda y actualiza la resolución de pantalla.
    /// </summary>
    void ApplySettings()
    {
        AudioManager.Instance.PlayClickSound();

        string[] selectedResolution = resolutionDropdown.options[resolutionDropdown.value].text.Split('x');
        int width = int.Parse(selectedResolution[0].Trim());
        int height = int.Parse(selectedResolution[1].Trim());
        bool isFullscreen = Screen.fullScreen;

        Screen.SetResolution(width, height, isFullscreen);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("AspectRatioSetting", aspectRatioDropdown.value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Regresa al panel del menú principal.
    /// </summary>
    void BackToMainMenu()
    {
        AudioManager.Instance.PlayClickSound();
        panelajustes.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Metodo para alternar entre pantalla completa y modo ventana.
    /// </summary>
    void ToggleFullscreen()
    {
        bool newState = !Screen.fullScreen;
        Screen.fullScreen = newState;
        AudioManager.Instance.PlayClickSound();
        PlayerPrefs.SetInt("IsFullscreen", newState ? 1 : 0);
        UpdateFullscreenButtonText(newState);
    }

    /// <summary>
    /// Metodo para actualizar el texto del botón de pantalla completa según el estado actual.
    /// </summary>
    void UpdateFullscreenButtonText(bool? state = null)
    {
        bool isFullscreen = state ?? Screen.fullScreen;
        if (isFullscreen)
            fullscreenButtonText.text = "Pantalla completa: Activado";
        else
            fullscreenButtonText.text = "Pantalla completa: Desactivado";
    }

    #region Control de Volumen

    /// <summary>
    /// Ajusta el volumen de la música en función del valor indicado.
    /// </summary>
    /// <param name="change">Valor de cambio en el volumen.</param>
    void ChangeMusicVolume(float change)
    {
        musicVolume = Mathf.Clamp(musicVolume + change, 0f, 1f);
        AudioManager.Instance.SetGeneralMusicVolume(musicVolume);
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Ajusta el volumen de los efectos sonoros en función del valor indicado.
    /// </summary>
    /// <param name="change">Valor de cambio en el volumen.</param>
    void ChangeSFXVolume(float change)
    {
        sfxVolume = Mathf.Clamp(sfxVolume + change, 0f, 1f);
        AudioManager.Instance.SetGeneralSFXVolume(sfxVolume);
        //AudioManager.Instance.PlayClickButtonVolumeSound();
        UpdateVolumeTexts();
    }

    /// <summary>
    /// Actualiza las etiquetas de texto del volumen para reflejar los ajustes actuales.
    /// </summary>
    void UpdateVolumeTexts()
    {
        musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100) + "%";
        sfxVolumeText.text = Mathf.RoundToInt(sfxVolume * 100) + "%";
    }

    #endregion
}
