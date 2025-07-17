using UnityEngine;

/// <summary>
/// Representa la informaci�n de un nivel, incluyendo el nombre mostrado en los botones 
/// y el nombre de la escena a cargar.
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    /// <summary>
    /// Nombre del nivel, a mostrar en botones o men�s.
    /// </summary>
    public string levelName; 
    
    /// <summary>
    /// Nombre de la escena que se cargar� para este nivel.
    /// </summary>
    public string sceneName; 
}
