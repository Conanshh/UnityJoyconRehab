using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject que almacena una lista de niveles disponibles en el juego.
/// Se utiliza para gestionar y mostrar niveles en selecciones de juego.
/// </summary>
[CreateAssetMenu(fileName = "AllLevels", menuName = "ScriptableObjects/LevelList", order = 2)]
public class LevelList : ScriptableObject
{
    /// <summary>
    /// Lista de datos de niveles.
    /// </summary>
    public List<LevelData> levels;
}
