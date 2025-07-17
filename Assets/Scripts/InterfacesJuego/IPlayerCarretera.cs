using TMPro;
using UnityEngine;

/// <summary>
/// Interfaz que define las operaciones básicas de un jugador en el modo Carretera.
/// Permite la inicialización con referencias a componentes del nivel, 
/// la adición de monedas y el manejo de colisiones.
/// </summary>
public interface IPlayerCarretera
{
    /// <summary>
    /// Inicializa el jugador con las referencias necesarias para el funcionamiento del nivel.
    /// </summary>
    /// <param name="tilemap">Objeto que representa el tilemap del nivel.</param>
    /// <param name="redOverlay">Objeto para la imágen en rojo.</param>
    /// <param name="obstacleSpawner">Instancia del spawner de obstáculos.</param>
    /// <param name="rewardSpawner">Instancia del spawner de recompensas.</param>
    /// <param name="spawnManager">Gestor de posiciones de spawn.</param>
    /// <param name="coinText">Texto que muestra la cantidad de monedas.</param>
    /// <param name="messageText">Texto para mostrar mensajes.</param>
    /// <param name="pausePanel">Panel de pausa del juego.</param>
    /// <param name="backText">Objeto de texto para mensajes tutoriales.</param>
    void Init(GameObject tilemap, GameObject redOverlay, ObstacleSpawner obstacleSpawner, RewardSpawner rewardSpawner,
              SpawnManager spawnManager, TextMeshProUGUI coinText, TextMeshProUGUI messageText,
              GameObject pausePanel, GameObject backText);
    /// <summary>
    /// Aumenta el contador de monedas del jugador.
    /// </summary>
    void AddCoin();

    /// <summary>
    /// Gestiona la colisión del jugador con un obstáculo.
    /// </summary>
    void HandleCollision();

    /// <summary>
    /// Aplica la lógica de pausa al jugador (por ejemplo, deteniendo animaciones o movimientos).
    /// </summary>
    void OnPauseGame();

    /// <summary>
    /// Reanuda la ejecución del jugador tras haberse aplicado la pausa.
    /// </summary>
    void OnResumeGame();
}
