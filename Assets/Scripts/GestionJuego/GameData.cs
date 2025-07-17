using System.IO;
using UnityEngine;

/// <summary>
/// Representa los datos del juego y provee métodos para cargar y guardar dichos datos en formato JSON.
/// </summary>
[System.Serializable]
public class GameData
{
    // Puntuaciones
    public int highScoreAbAd = 0;
    public int highScoreFE = 0;

    // Recolección de monedas y gemas
    public int totalCoins = 0;
    public int totalGems = 0;

    // Umbrales
    public float thresholdAngleAbAd = 20f;
    public float thresholdAngleFE = 20f;

    // Preferencias del usuario
    public bool isLeftJoyCon = true;
    public int GameTime = 60;

    // Selección de personajes
    public string characterAbAd;
    public string characterFE;

    /// <summary>
    /// Ruta completa del archivo JSON donde se almacenan los datos del juego.
    /// </summary>
    private static string filePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    /// <summary>
    /// Carga los datos del juego desde un archivo JSON.
    /// Si el archivo no existe, retorna una nueva instancia de GameData con valores por defecto.
    /// </summary>
    /// <returns>Una instancia de GameData con los datos cargados o con valores por defecto.</returns>
    public static GameData Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return new GameData();
    }

    /// <summary>
    /// Guarda los datos actuales del juego en un archivo JSON.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(filePath, json);
    }
}