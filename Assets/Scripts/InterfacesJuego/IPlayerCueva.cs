using TMPro;
using UnityEngine;

/// <summary>
/// Interfaz que define las operaciones básicas de un jugador en el modo Cueva.
/// Se encarga de inicializar al jugador con las referencias necesarias, incrementar 
/// el contador de gemas y manejar los estados de pausa y reanudación.
/// </summary>
public interface IPlayerCueva
{
    /// <summary>
    /// Inicializa al jugador con las referencias necesarias para el funcionamiento en el modo Cueva.
    /// </summary>
    /// <param name="gemText">Texto que muestra la cantidad de gemas recolectadas.</param>
    /// <param name="gemsSpawner">Instancia del gestor de gemas.</param>
    /// <param name="pausePanel">Panel de pausa del juego.</param>
    /// <param name="backText">Objeto de texto para mensajes de retroalimentación.</param>
    /// <param name="feedbackText">Objeto de texto adicional para feedback.</param>
    /// <param name="camera">Componente para efectos de vibración de la cámara.</param>
    /// <param name="redOverlay">Objeto que representa la superposición en rojo.</param>
    void Init(
        TextMeshProUGUI gemText,
        GemsSpawner gemsSpawner,
        GameObject pausePanel,
        GameObject backText,
        GameObject feedbackText,
        CameraShake camera,
        GameObject redOverlay);

    /// <summary>
    /// Incrementa el contador de gemas recolectadas y actualiza la interfaz del jugador.
    /// </summary>
    void AddGem();

    /// <summary>
    /// Aplica la lógica necesaria del jugador al pausarse el nivel(por ejemplo, detener animaciones o movimientos).
    /// </summary>
    void OnGamePaused();

    /// <summary>
    /// Reanuda la ejecución del jugador tras haber sido pausado.
    /// </summary>
    void OnGameResumed();
}
