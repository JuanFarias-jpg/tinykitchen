using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class pausayeso : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject Mouse;

    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private PlayerCombat playerCombat;

    [SerializeField] private float resumeAttackDelay = 0.15f;

    // Variables SO para guardar el estado antes de ir a Personalizacion.
    [Header("Variables SO — para guardar estado")]
    [SerializeField] private FloatVariable gameTimer;
    [SerializeField] private IntVariable playerHP;

    private PlayerControls controls;
    private bool juegoPausado = false;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        controls.Gameplay.Pause.performed -= OnPause;
        controls.Gameplay.Disable();
    }

    private void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (juegoPausado)
            Reanudar();
        else
            Pausa();
    }

    public void Pausa()
    {
        juegoPausado = true;
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);

        if (Mouse != null) Mouse.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    public void Reanudar()
    {
        juegoPausado = false;
        menuPausa.SetActive(false);
        botonPausa.SetActive(true);

        if (Mouse != null) Mouse.SetActive(true);

        Time.timeScale = 1f;

        if (playerCombat != null)
            playerCombat.enabled = false;

        StartCoroutine(RehabilitarCombate());

        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    private IEnumerator RehabilitarCombate()
    {
        yield return null;

        if (playerInput != null)
            playerInput.ConsumeAttack();

        yield return new WaitForSeconds(resumeAttackDelay);

        if (playerCombat != null)
            playerCombat.enabled = true;
    }

    public void Options()
    {
        // Guardar el estado actual en PlayerPrefs antes de cargar Personalizacion.
        // GameManager.cs lo lee al volver y restaura los valores en los SOs.
        if (gameTimer != null)
            PlayerPrefs.SetFloat("SavedTimer", gameTimer.Value);
        if (playerHP != null)
            PlayerPrefs.SetInt("SavedHP", playerHP.Value);

        // Guardar flag para que GameManager sepa que viene de una pausa
        // y no resetee los valores con ResetAllVariables().
        PlayerPrefs.SetInt("ComingFromPersonalizacion", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        juegoPausado = false;
        SceneManager.LoadScene("Personalizacion");
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cerrar()
    {
        Application.Quit();
    }
}