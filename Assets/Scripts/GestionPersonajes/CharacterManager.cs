using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Clase responsable de gestionar la selección e instanciación de personajes en el juego.
/// Maneja la lista de datos de personajes y permite activar el prefab correspondiente al personaje seleccionado.
/// Utiliza el patrón Singleton para garantizar una única instancia a lo largo de todas las escenas.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton del CharacterManager.
    /// </summary>
    public static CharacterManager Instance { get; private set; }

    /// <summary>
    /// Diccionario que almacena el personaje seleccionado para cada tipo de nivel.
    /// </summary>
    private Dictionary<NivelTipo, CharacterData> selectedCharacters = new();

    /// <summary>
    /// Personaje por defecto para el nivel Carretera (Abducción-Aducción) si no se ha seleccionado ninguno.
    /// </summary>
    public CharacterData defaultCharacterCarretera;

    /// <summary>
    /// Personaje por defecto para el nivel Cueva (Flexión-Extensión) si no se ha seleccionado ninguno.
    /// </summary>
    public CharacterData defaultCharacterCueva;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Se carga el personaje previamente elegido desde GameData
        GameData game = GameData.Load();

        // Se intenta obtener el CharacterData de acuerdo al ID guardado y el tipo de nivel.
        CharacterData personajeCarretera = FindCharacterByID(game.characterAbAd, NivelTipo.Carretera);
        CharacterData personajeCueva = FindCharacterByID(game.characterFE, NivelTipo.Cueva);

        // Se asignan los personajes seleccionados o se fallback al personaje por defecto si no hay información.
        selectedCharacters[NivelTipo.Carretera] = personajeCarretera ?? defaultCharacterCarretera;
        selectedCharacters[NivelTipo.Cueva] = personajeCueva ?? defaultCharacterCueva;
    }

    /// <summary>
    /// Busca y retorna un CharacterData basado en un identificador y el tipo de nivel.
    /// </summary>
    /// <param name="id">Identificador o nombre del personaje.</param>
    /// <param name="tipo">Tipo de nivel (por ejemplo, Carretera o Cueva).</param>
    /// <returns>
    /// El CharacterData que coincida con el identificador y tipo, o null si no se encuentra.
    /// </returns>
    private CharacterData FindCharacterByID(string id, NivelTipo tipo)
    {
        // Se obtiene la referencia de CharacterList que contiene la lista de personajes.
        var lista = FindFirstObjectByType<CharacterList>();
        if (lista == null) return null;

        // Se busca un personaje que tenga el mismo nombre y el tipo indicado.
        return lista.characters.Find(c => c.characterName == id && c.nivelTipo == tipo);
    }

    /// <summary>
    /// Asigna el personaje seleccionado para un tipo de nivel específico.
    /// </summary>
    /// <param name="tipo">Tipo de nivel en el cual se realiza la selección.</param>
    /// <param name="data">Datos del personaje seleccionado.</param>
    public void SetSelectedCharacter(NivelTipo tipo, CharacterData data)
    {
        selectedCharacters[tipo] = data;
    }

    /// <summary>
    /// Obtiene el CharacterData seleccionado para el tipo de nivel especificado.
    /// </summary>
    /// <param name="tipo">Tipo de nivel para el cual se requiere el personaje.</param>
    /// <returns>
    /// Devuelve el CharacterData asignado para el tipo de nivel, o null en caso de que no exista.
    /// </returns>
    public CharacterData GetSelectedCharacter(NivelTipo tipo)
    {
        return selectedCharacters.TryGetValue(tipo, out var character) ? character : null;
    }

    /// <summary>
    /// Retorna el CharacterData por defecto asignado para un tipo de nivel dado.
    /// </summary>
    /// <param name="tipo">Tipo de nivel (Carretera o Cueva).</param>
    /// <returns>
    /// Devuelve el CharacterData por defecto para el tipo de nivel; en caso de tipo desconocido se retorna null.
    /// </returns>
    public CharacterData GetDefaultCharacter(NivelTipo tipo)
    {
        return tipo switch
        {
            NivelTipo.Carretera => defaultCharacterCarretera,
            NivelTipo.Cueva => defaultCharacterCueva,
            _ => null,
        };
    }
}
