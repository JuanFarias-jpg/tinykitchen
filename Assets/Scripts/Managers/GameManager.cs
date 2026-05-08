using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Paused, Won, Lost }
    public GameState Estado { get; private set; } = GameState.Playing;

    [Header("Variables SO")]
    public IntVariable playerHP;
    public FloatVariable gameTimer;
    public IntVariable bossHP;
    public FloatVariable freezeLevel;

    [Header("Eventos SO")]
    public GameEvent OnBossDefeated;
    public GameEvent OnTimeExpired;

    [Header("UI")]
    public WinLoseScreen winLoseScreen;

    // bandera para evitar reset al volver de Personalizacion
    public static bool volverDePersonalizacion = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // SOLO resetear si NO viene de Personalizacion
        if (!volverDePersonalizacion)
        {
            ResetAllVariables();
            PlayerHealth.IsDead = false;
        }

        volverDePersonalizacion = false;
    }

    private void ResetAllVariables()
    {
        if (playerHP != null) playerHP.ResetToInitial();
        if (gameTimer != null) gameTimer.ResetToInitial();
        if (bossHP != null) bossHP.ResetToInitial();
        if (freezeLevel != null) freezeLevel.ResetToInitial();
    }

    public void WinGame()
    {
        if (Estado != GameState.Playing) return;

        Estado = GameState.Won;
        Time.timeScale = 0f;

        if (winLoseScreen != null)
            winLoseScreen.ShowWin();
    }

    public void LoseGame()
    {
        if (Estado != GameState.Playing) return;

        Estado = GameState.Lost;
        Time.timeScale = 0f;

        if (winLoseScreen != null)
            winLoseScreen.ShowLose();
    }

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

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }
}