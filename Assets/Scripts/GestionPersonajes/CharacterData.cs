using UnityEngine;

/// <summary>
/// ScriptableObject que contiene la información y configuración de un personaje en el juego.
/// Permite asignar el nombre, prefab, imagen de previsualización y el tipo de nivel asociado al personaje.
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// Nombre del personaje.
    /// </summary>
    public string characterName;

    /// <summary>
    /// Prefab del personaje que se instanciará en la escena.
    /// </summary>
    public GameObject characterPrefab;

    /// <summary>
    /// Imagen de previsualización del personaje, utilizada en la interfaz de usuario.
    /// </summary>
    public Sprite previewImage;

    /// <summary>
    /// Tipo de nivel donde se utiliza este personaje (por ejemplo, Carretera o Cueva).
    /// </summary>
    public NivelTipo nivelTipo;
}