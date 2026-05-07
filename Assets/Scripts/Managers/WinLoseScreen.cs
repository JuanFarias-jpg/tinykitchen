using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinLoseScreen : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    [Header("Textos (opcionales)")]
    public TextMeshProUGUI textoVictoria;
    public TextMeshProUGUI textoDerrota;

    [Header("Nombre de la escena del menú principal")]
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota  != null) panelDerrota.SetActive(false);
    }

    public void ShowWin()
    {
        if (panelVictoria != null) panelVictoria.SetActive(true);
        if (panelDerrota  != null) panelDerrota.SetActive(false);

        // Mostrar cursor para que el jugador pueda hacer clic en los botones.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void ShowLose()
    {
        if (panelDerrota  != null) panelDerrota.SetActive(true);
        if (panelVictoria != null) panelVictoria.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    // ─── Botones ──────────────────────────────────────────────────────────────
    public void OnBotonReintentar()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
        else
        {
            // Fallback si GameManager no está disponible.
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
    public void OnBotonMenuPrincipal()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}