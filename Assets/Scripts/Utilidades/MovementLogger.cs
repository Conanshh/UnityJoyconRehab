using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Datos de un movimiento individual.
/// </summary>
[Serializable]
public class MovementData
{
    public string movementType;
    public float Angle;
    public float thresholdAngle;
    public float timeStamp; // Timestamp para registrar el momento del movimiento
}

/// <summary>
/// Datos de una sesión (por ejemplo, una partida) para un modo determinado.
/// </summary>
[Serializable]
public class SessionData
{
    public float thresholdAngle;
    public string axisUsed;
    public int totalMovements;
    public float assignedGameTime;
    public float totalGameTimeAbAd;
    public float totalGameTimeFE;
    public int rewardCount;
    public string characterID;
    public string joyconUsed; // "izquierdo" o "derecho"
    public List<MovementData> movements = new List<MovementData>();
}

/// <summary>
/// Información completa de la sesión, que agrupa datos para cada modo.
/// </summary>
[Serializable]
public class FullSessionData
{
    public string date;
    public string userName;
    public SessionData abduccionAduccion;
    public SessionData flexionExtension;
}

/// <summary>
/// Datos acumulados de un usuario, usados para persistir high scores y sesiones anteriores.
/// </summary>
[Serializable]
public class AccumulatedUserData
{
    public string userName;
    public string date;
    public int highScoreAbAd = 0;
    public int highScoreFE = 0;
    public List<SessionData> flexionExtension = new List<SessionData>();
    public List<SessionData> abduccionAduccion = new List<SessionData>();
}

/// <summary>
/// Gestiona el registro de movimientos y sesiones de juego, permitiendo guardar y
/// cargar datos de rendimiento y configuraciones para cada usuario.
/// </summary>
public class MovementLogger : MonoBehaviour
{
    #region Singleton

    public static MovementLogger Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartNewSession();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Datos de Sesión

    [SerializeField]
    private FullSessionData fullSessionData = new FullSessionData();
    public FullSessionData FullSessionData => fullSessionData;

    // Indica cuál fue el último modo usado (FE: modo Cueva, AbAd: modo Carretera)
    public bool lastWasFE;

    private float sessionStartTimeFE;
    private float sessionStartTimeAbAd;
    private string currentUserFilePath;

    #endregion

    #region Métodos de Carga de Datos

    /// <summary>
    /// Carga los high scores más recientes para el usuario suministrado.
    /// Si no existen datos previos, se inicializan a 0.
    /// </summary>
    /// <param name="userName">Nombre del usuario.</param>
    public void LoadLatestHighScores(string userName)
    {
        string folderPath = Application.persistentDataPath;
        string[] files = Directory.GetFiles(folderPath, $"{userName}*.json");

        GameData game = GameData.Load();
        if (files.Length == 0)
        {
            game.highScoreAbAd = 0;
            game.highScoreFE = 0;
            game.Save();
            return;
        }
        // Ordenar por fecha descendente
        Array.Sort(files, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));
        string latestFile = files[0];
        string json = File.ReadAllText(latestFile);
        AccumulatedUserData data = JsonUtility.FromJson<AccumulatedUserData>(json);
        game.highScoreAbAd = data.highScoreAbAd;
        game.highScoreFE = data.highScoreFE;
        game.Save();
    }

    /// <summary>
    /// Carga los IDs de personajes más recientes para el usuario en función de las sesiones guardadas.
    /// </summary>
    /// <param name="userName">Nombre del usuario.</param>
    public void LoadLatestCharacterIDs(string userName)
    {
        string folderPath = Application.persistentDataPath;
        string[] files = Directory.GetFiles(folderPath, $"{userName}*.json");

        GameData game = GameData.Load();
        if (files.Length == 0)
        {
            game.characterAbAd = null;
            game.characterFE = null;
            game.Save();
            return;
        }
        Array.Sort(files, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));
        string latestFile = files[0];
        string json = File.ReadAllText(latestFile);
        AccumulatedUserData data = JsonUtility.FromJson<AccumulatedUserData>(json);

        if (data.abduccionAduccion != null && data.abduccionAduccion.Count > 0)
        {
            game.characterAbAd = data.abduccionAduccion[data.abduccionAduccion.Count - 1].characterID;
        }
        if (data.flexionExtension != null && data.flexionExtension.Count > 0)
        {
            game.characterFE = data.flexionExtension[data.flexionExtension.Count - 1].characterID;
        }
        game.Save();
    }

    #endregion

    #region Gestión de Sesión y Movimientos

    /// <summary>
    /// Inicia una nueva sesión asignando la fecha actual y reiniciando las sesiones para cada modo.
    /// </summary>
    private void StartNewSession()
    {
        fullSessionData.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        fullSessionData.abduccionAduccion = new SessionData();
        fullSessionData.flexionExtension = new SessionData();
    }

    /// <summary>
    /// Configura la sesión para el modo especificado (FE o AbAd) con los parámetros dados.
    /// </summary>
    public void SetSessionInfo(bool isFE, float thresholdAngle, string axis, float gameTime, bool joyconIsLeft)
    {
        var session = isFE ? fullSessionData.flexionExtension : fullSessionData.abduccionAduccion;
        session.thresholdAngle = thresholdAngle;
        session.axisUsed = axis;
        session.assignedGameTime = gameTime;
        session.joyconUsed = joyconIsLeft ? "izquierdo" : "derecho";
    }

    /// <summary>
    /// Registra un movimiento para el modo de flexión/extensión.
    /// </summary>
    public void RegisterFlexionExtensionMovement(string movementType, float angle, float thresholdAngle, float timestamp)
    {
        var session = fullSessionData.flexionExtension;
        session.movements.Add(new MovementData
        {
            movementType = movementType,
            Angle = angle,
            thresholdAngle = thresholdAngle,
            timeStamp = timestamp,

        });
        session.totalMovements++;
    }

    /// <summary>
    /// Registra un movimiento para el modo de abducción/aducción.
    /// </summary>
    public void RegisterAbductionAdductionMovement(string movementType, float angle, float thresholdAngle, float timestamp)
    {
        var session = fullSessionData.abduccionAduccion;
        session.movements.Add(new MovementData
        {
            movementType = movementType,
            Angle = angle,
            thresholdAngle = thresholdAngle,
            //timeStamp = Time.time - sessionStartTimeAbAd,
            timeStamp = timestamp,
        });
        session.totalMovements++;
    }

    /// <summary>
    /// Establece la cantidad de recompensas obtenidas, diferenciando según el modo.
    /// </summary>
    public void SetRewardCount(int count, bool isFE)
    {
        if (isFE)
            fullSessionData.flexionExtension.rewardCount = count;
        else
            fullSessionData.abduccionAduccion.rewardCount = count;
    }

    /// <summary>
    /// Configura el usuario actual y prepara la ruta de archivo para guardar la sesión.
    /// También carga los high scores previos para ese usuario.
    /// </summary>
    public void SetUser(string name)
    {
        StartNewSession();
        fullSessionData.userName = name;
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmm");
        string fileName = $"{name}_{timestamp}.json";
        currentUserFilePath = Path.Combine(Application.persistentDataPath, fileName);
        LoadLatestHighScores(name);
    }

    /// <summary>
    /// Reinicia el temporizador de la sesión según el modo.
    /// </summary>
    public void ResetTimer(bool isFE)
    {
        if (isFE)
        {
            sessionStartTimeFE = Time.time;
        }
        else
        {
            sessionStartTimeAbAd = Time.time;
        }
        lastWasFE = isFE;
    }

    /// <summary>
    /// Guarda los datos de la sesión actual en un archivo JSON y actualiza el high score si corresponde.
    /// Luego, inicia una nueva sesión.
    /// </summary>
    public void SaveSession()
    {
        // Calcular tiempo total de sesión
        if (lastWasFE && fullSessionData.flexionExtension.totalMovements > 0)
        {
            float totalTimeFE = Time.time - sessionStartTimeFE;
            fullSessionData.flexionExtension.totalGameTimeFE = totalTimeFE;
        }
        else if (!lastWasFE && fullSessionData.abduccionAduccion.totalMovements > 0)
        {
            float totalTimeAbAd = Time.time - sessionStartTimeAbAd;
            fullSessionData.abduccionAduccion.totalGameTimeAbAd = totalTimeAbAd;
        }

        // Cargar datos previos (si existen) para acumular información del usuario
        GameData game = GameData.Load();
        AccumulatedUserData dataToSave;
        if (File.Exists(currentUserFilePath))
        {
            string existingJson = File.ReadAllText(currentUserFilePath);
            dataToSave = JsonUtility.FromJson<AccumulatedUserData>(existingJson);
        }
        else
        {
            dataToSave = new AccumulatedUserData
            {
                userName = fullSessionData.userName,
                date = fullSessionData.date,
                highScoreFE = game.highScoreFE,
                highScoreAbAd = game.highScoreAbAd
            };
        }

        // Obtener el personaje actual seleccionado (según el modo)
        CharacterData currentChar = CharacterManager.Instance.GetSelectedCharacter(lastWasFE ? NivelTipo.Cueva : NivelTipo.Carretera);
        string characterNameUsed = currentChar != null ? currentChar.characterName : "Default";

        if (lastWasFE && fullSessionData.flexionExtension.totalMovements > 0)
        {
            fullSessionData.flexionExtension.characterID = characterNameUsed;
            dataToSave.flexionExtension.Add(fullSessionData.flexionExtension);
        }
        else if (!lastWasFE && fullSessionData.abduccionAduccion.totalMovements > 0)
        {
            fullSessionData.abduccionAduccion.characterID = characterNameUsed;
            dataToSave.abduccionAduccion.Add(fullSessionData.abduccionAduccion);
        }

        // Actualizar high scores si corresponde
        int currentScore = lastWasFE ? fullSessionData.flexionExtension.rewardCount : fullSessionData.abduccionAduccion.rewardCount;
        if (lastWasFE)
        {
            if (currentScore > game.highScoreFE)
            {
                dataToSave.highScoreFE = currentScore;
                game.highScoreFE = currentScore;
            }
        }
        else
        {
            if (currentScore > game.highScoreAbAd)
            {
                dataToSave.highScoreAbAd = currentScore;
                game.highScoreAbAd = currentScore;
            }
        }
        game.Save();

        // Guardar el JSON final
        string finalJson = JsonUtility.ToJson(dataToSave, true);
        File.WriteAllText(currentUserFilePath, finalJson);

        // Iniciar una nueva sesión para el mismo usuario
        StartNewSession();
    }

    #endregion
}