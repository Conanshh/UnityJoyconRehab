using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject que contiene una lista de CharacterData para gestionar los personajes disponibles en el juego.
/// Permite organizar y acceder a la información de cada personaje de manera centralizada.
/// </summary>
[CreateAssetMenu(fileName = "CharacterList", menuName = "Scriptable Objects/CharacterList")]
public class CharacterList : ScriptableObject
{
    /// <summary>
    /// Lista de datos de personajes disponibles.
    /// Cada elemento en la lista contiene la configuración e información de un personaje.
    /// </summary>
    public List<CharacterData> characters;
}
