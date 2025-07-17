using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Clase responsable de gestionar la selección de personajes a través de la interfaz de usuario.
/// Filtra la lista de personajes según el tipo de nivel, permite navegar entre ellos y confirmar la selección.
/// </summary>
public class CharacterSelector : MonoBehaviour
{
    /// <summary>
    /// Lista de personajes disponibles.
    /// </summary>
    public CharacterList characterList;

    /// <summary>
    /// Imagen de previsualización del personaje.
    /// </summary>
    public Image previewImage;

    /// <summary>
    /// Texto que muestra el nombre del personaje en la interfaz.
    /// </summary>
    public TMP_Text characterNameText;

    /// <summary>
    /// Botón para avanzar al siguiente personaje.
    /// </summary>
    public Button nextButton;

    /// <summary>
    /// Botón para retroceder al personaje anterior.
    /// </summary>
    public Button previousButton;

    /// <summary>
    /// Botón para confirmar la selección del personaje.
    /// </summary>
    public Button confirmButton;

    /// <summary>
    /// Panel del menú principal que se muestra luego de seleccionar un personaje.
    /// </summary>
    public GameObject panelMenu;

    /// <summary>
    /// Panel de selección de personaje.
    /// </summary>
    public GameObject panelCharacter;


    /// <summary>
    /// Índice actual del personaje seleccionado en la lista filtrada.
    /// </summary>
    private int currentIndex = 0;

    /// <summary>
    /// Lista filtrada de personajes basados en el tipo de nivel.
    /// </summary>
    private List<CharacterData> personajesFiltrados;

    /// <summary>
    /// Tipo de nivel para el cual se filtran los personajes. Por defecto es Carretera.
    /// </summary>
    public NivelTipo nivelTipo = NivelTipo.Carretera;

    /// <summary>
    /// Inicializa el selector de personajes.
    /// Filtra la lista de personajes, carga la selección guardada y configura los eventos de los botones.
    /// </summary>
    void Start()
    {
        // Se filtra la lista de personajes según el tipo de nivel seleccionado.
        personajesFiltrados = characterList.characters.FindAll(c => c.nivelTipo == nivelTipo);
        if (personajesFiltrados.Count == 0)
        {
            Debug.LogError("No hay personajes disponibles para este tipo de nivel.");
            return;
        }

        // Se carga la información guardada del juego para recuperar la selección previa.
        GameData game = GameData.Load();

        // Se obtiene el identificador del personaje guardado según el tipo de nivel.
        string savedCharacterID = nivelTipo == NivelTipo.Carretera ? game.characterAbAd : game.characterFE;

        // Se busca el personaje guardado dentro de la lista filtrada.
        int index = personajesFiltrados.FindIndex(p => p.characterName == savedCharacterID);
        currentIndex = (index != -1) ? index : 0;  // Si no se encuentra, usar 0 como default

        // Se actualiza la interfaz con los datos del personaje seleccionado.
        UpdateUI();

        // Configuración del botón para avanzar al siguiente personaje.
        nextButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickButtonSelectorNextSound();
            currentIndex = (currentIndex + 1) % personajesFiltrados.Count;
            UpdateUI();
        });

        // Configuración del botón para retroceder al personaje anterior.
        previousButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickButtonSelectorBackSound();
            currentIndex = (currentIndex - 1 + personajesFiltrados.Count) % personajesFiltrados.Count;
            UpdateUI();
        });

        // Configuración del botón para confirmar la selección del personaje.
        confirmButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound();
            CharacterManager.Instance.SetSelectedCharacter(nivelTipo, personajesFiltrados[currentIndex]);
            Debug.Log("Personaje elegido: " + personajesFiltrados[currentIndex].characterName);
            panelCharacter.SetActive(false);
            panelMenu.SetActive(true);
        });
    }

    /// <summary>
    /// Actualiza la interfaz de usuario mostrando la imagen y el nombre del personaje actualmente seleccionado.
    /// </summary>
    void UpdateUI()
    {
        var data = personajesFiltrados[currentIndex];
        previewImage.sprite = data.previewImage;
        characterNameText.text = data.characterName;
    }
}
