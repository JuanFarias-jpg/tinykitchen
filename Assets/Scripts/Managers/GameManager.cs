using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Paused, Won, Lost }
    public GameState Estado { get; private set; } = GameState.Playing;

    [Header("Variables SO — asignar en Inspector")]
    [Tooltip("HP del jugador. Se resetea a initialValue al empezar.")]
    public IntVariable   playerHP;
    [Tooltip("Timer del juego. initialValue debe ser 1200 (20 min).")]
    public FloatVariable gameTimer;
    [Tooltip("HP del boss. Se resetea a initialValue al empezar.")]
    public IntVariable   bossHP;
    [Tooltip("Nivel de congelamiento del jugador. Se resetea a 0.")]
    public FloatVariable freezeLevel;

    [Header("Eventos SO — asignar en Inspector")]
    [Tooltip("Escucha este evento para transición a victoria.")]
    public GameEvent OnBossDefeated;
    [Tooltip("Escucha este evento para transición a derrota.")]
    public GameEvent OnTimeExpired;

    [Header("UI")]
    [Tooltip("Pantalla de victoria/derrota. Asignar en el Inspector.")]
    public WinLoseScreen winLoseScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetAllVariables();

        // Evita que IsDead quede true entre sesiones de Play en el editor
        PlayerHealth.IsDead = false;
    }

    private void ResetAllVariables()
    {
        // Cada SO tiene ResetToInitial() que copia initialValue → runtimeValue.
        if (playerHP    != null) playerHP.ResetToInitial();
        if (gameTimer   != null) gameTimer.ResetToInitial();
        if (bossHP      != null) bossHP.ResetToInitial();
        if (freezeLevel != null) freezeLevel.ResetToInitial();
    }

    // Llamado por el GameEventListener de OnBossDefeated.
    public void WinGame()
    {
        if (Estado != GameState.Playing) return;
        Estado = GameState.Won;
        Time.timeScale = 0f;
        if (winLoseScreen != null) winLoseScreen.ShowWin();
    }

    // Llamado por el GameEventListener de OnTimeExpired, o por PlayerHealth.Die().
    public void LoseGame()
    {
        if (Estado != GameState.Playing) return;
        Estado = GameState.Lost;
        Time.timeScale = 0f;
        if (winLoseScreen != null) winLoseScreen.ShowLose();
    }

    // Llamado por pausayeso.cs cuando el jugador presiona Escape.
    public void TogglePause()
    {
        if (Estado == GameState.Playing)
        {
            Estado = GameState.Paused;
            Time.timeScale = 0f;
        }
        else if (Estado == GameState.Paused)
        {
            Estado = GameState.Playing;
            Time.timeScale = 1f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        PlayerAnimator.ResetLevantarse();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}