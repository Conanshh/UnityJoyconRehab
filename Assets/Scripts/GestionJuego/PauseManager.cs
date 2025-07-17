using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la pausa global del juego, permitiendo pausar y reanudar el flujo del juego.
/// Controla la escala de tiempo e informa a otros sistemas mediante eventos.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static event Action OnPause;  // Evento que se lanza al pausar el juego
    public static event Action OnResume; // Evento que se lanza al reanudar el juego

    [Header("UI Elements")]
    public GameObject pausePanel;
    public GameObject backText;

    void Start()
    {
        // Suscribirse al evento OnRequestPause del PlayerCaveMovement, solo para caso Cave.
        PlayerCaveMovement pcm = FindFirstObjectByType<PlayerCaveMovement>();
        if (pcm != null)
        {
            pcm.OnRequestPause += PauseGame;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// Metodo para pausar el juego, desactivando la música y mostrando el panel de pausa.
    /// </summary>
    public void PauseGame()
    {
        AudioManager.Instance.PauseMusic();
        if (backText != null)
            backText.SetActive(false);

        pausePanel.SetActive(true);
        Time.timeScale = 0;
        OnPause?.Invoke(); // Lanzar evento
    }

    /// <summary>
    /// Metodo para reanudar el juego, reactivando la música y ocultando el panel de pausa.
    /// </summary>
    public void ResumeGame()
    {
        AudioManager.Instance.UnpauseMusic();
        pausePanel.SetActive(false);
        if (backText != null)
            backText.SetActive(true);

        Time.timeScale = 1;
        OnResume?.Invoke(); // Lanzar evento
    }

    /// <summary>
    /// Desuscribe del evento OnRequestPause del PlayerCaveMovement al destruir el objeto.
    /// </summary>
    void OnDestroy()
    {
        PlayerCaveMovement pcm = FindFirstObjectByType<PlayerCaveMovement>();
        if (pcm != null)
        {
            pcm.OnRequestPause -= PauseGame;
        }
    }
}