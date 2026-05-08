using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Paused, Won, Lost }
    public GameState Estado { get; private set; } = GameState.Playing;

    [Header("Variables SO — asignar en Inspector")]
    public IntVariable playerHP;
    public FloatVariable gameTimer;
    public IntVariable bossHP;
    
    public FloatVariable freezeLevel;

    [Header("Eventos SO — asignar en Inspector")]
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

        // Si viene de Personalizacion, restaurar el estado guardado
        // en lugar de resetear todo desde initialValue.
        if (PlayerPrefs.GetInt("ComingFromPersonalizacion", 0) == 1)
        {
            RestaurarEstadoGuardado();
            PlayerPrefs.DeleteKey("ComingFromPersonalizacion");
        }
        else
        {
            ResetAllVariables();
        }

        PlayerHealth.IsDead = false;
    }

    private void ResetAllVariables()
    {
        if (playerHP    != null) playerHP.ResetToInitial();
        if (gameTimer   != null) gameTimer.ResetToInitial();
        if (bossHP      != null) bossHP.ResetToInitial();
        if (freezeLevel != null) freezeLevel.ResetToInitial();
    }

    // Restaura el timer y HP que se guardaron antes de ir a Personalizacion.
    // Los demás valores (bossHP, freezeLevel) se resetean normal porque
    // no cambian durante la sesión de forma que valga la pena guardar.
    private void RestaurarEstadoGuardado()
    {
        if (gameTimer != null)
        {
            gameTimer.ResetToInitial();
            gameTimer.Value = PlayerPrefs.GetFloat("SavedTimer", gameTimer.Value);
            PlayerPrefs.DeleteKey("SavedTimer");
        }

        if (playerHP != null)
        {
            playerHP.ResetToInitial();
            playerHP.Value = PlayerPrefs.GetInt("SavedHP", playerHP.Value);
            PlayerPrefs.DeleteKey("SavedHP");
        }

        if (bossHP      != null) bossHP.ResetToInitial();
        if (freezeLevel != null) freezeLevel.ResetToInitial();
    }

    public void WinGame()
    {
        if (Estado != GameState.Playing) return;


        Estado = GameState.Won;
        

        SceneManager.LoadScene("Win");
    }

    public void LoseGame()
    {
        if (Estado != GameState.Playing) return;


        Estado = GameState.Lost;
        SceneManager.LoadScene("Perder");
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}