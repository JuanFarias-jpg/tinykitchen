using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerHealth — Maneja la vida, daño, invincibilidad y muerte del jugador.
/// VERSIÓN CORREGIDA (Fase 1, Entrega Final).
///
/// CAMBIOS RESPECTO A LA VERSIÓN ANTERIOR:
///   ✓ IsDead = false en Awake() → soluciona el problema #4 de la bitácora.
///     Sin este fix, si el jugador moría y se presionaba Play de nuevo en el editor,
///     IsDead permanecía true y el jugador era intocable.
///   ✓ Método Heal(int) añadido → Queso.cs lo llama directamente (soluciona problema #3).
///   ✓ Die() notifica a GameManager.LoseGame() cuando existe (Fase 1).
///
/// FLUJO DE DAÑO UNIFICADO (problema #2 de la bitácora):
///   Todos los GameEventListeners de daño → PlayerHealth.TakeDamage()
///   El HUD solo lee playerHP.Value para mostrar corazones.
///   GameHUDManager.TakeDamage() fue eliminado de esa clase.
///
/// QUIÉN LLAMA A CADA MÉTODO:
///   TakeDamage() → GameEventListeners en los enemigos, FreezeZone, Estufa, BossCollider.
///   Heal()       → Queso.cs en OnTriggerEnter.
///   Die()        → llamado internamente cuando playerHP <= 0.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Variables")]
    public IntVariable playerHP;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    public int damageAmount  = 1;
    public float invincibilityTime = 1f;
    public int maxHP = 5;

    [Header("Parpadeo")]
    public float blinkInterval = 0.2f;
    private Renderer[] renderers;

    [Header("Audios")]
    public AudioSource Damage;
    public AudioSource Dead;

    private bool isInvincible = false;

    // static: compartido entre instancias y persiste entre Play/Stop en el editor.
    // CORRECCIÓN: se resetea en Awake() para evitar el bug de estado sucio.
    public static bool IsDead = false;

    private void Awake()
    {
        // Resetear IsDead aquí soluciona el problema #4 de la bitácora:
        // en el editor, cada vez que se presiona Play se ejecuta Awake() de nuevo,
        // limpiando el flag que quedó de la sesión anterior.
        IsDead = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        renderers = GetComponentsInChildren<Renderer>();
    }

    /// Aplica daño al jugador. Respeta invincibilidad.
    /// QUIÉN LO LLAMA: cualquier GameEventListener configurado para daño
    /// (enemigos, Estufa, FreezeZone, BossCollider).
    /// IMPORTANTE: este es el único punto donde playerHP.Value se reduce.
    public void TakeDamage()
    {
        if (isInvincible || IsDead) return;

        playerHP.Value -= damageAmount;
        playerHP.Value = Mathf.Max(0, playerHP.Value);

        if (Damage != null) Damage.Play();

        if (playerHP.Value <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFrames());
    }

    /// <summary>
    /// Cura al jugador.
    /// QUIÉN LO LLAMA: Queso.cs en OnTriggerEnter.
    /// Reemplaza la llamada anterior a GameHUDManager.Heal() (problema #3 de la bitácora).
    /// El HUD refleja el cambio automáticamente al leer playerHP.Value en Update.
    /// </summary>
    public void Heal(int amount)
    {
        if (IsDead) return;

        playerHP.Value += amount;
        playerHP.Value = Mathf.Clamp(playerHP.Value, 0, maxHP);
    }

    private void Die()
    {
        IsDead = true;

        if (Dead != null) Dead.Play();

        // Desactivar todos los sistemas del jugador.
        PlayerAnimator pa = GetComponent<PlayerAnimator>();
        if (pa != null) pa.enabled = false;

        PlayerMovement movimiento = GetComponent<PlayerMovement>();
        if (movimiento != null) movimiento.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null) combat.enabled = false;

        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        if (animator != null)
        {
            animator.CrossFade("muerteChef", 0f);
            animator.SetBool("IsDead", true);
        }

        // Notificar al GameManager para mostrar pantalla de derrota.
        // GameManager.Instance puede ser null si el script no está en escena todavía.
        if (GameManager.Instance != null)
            GameManager.Instance.LoseGame();

        Destroy(gameObject, 3.5f);
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            SetRenderers(false);
            yield return new WaitForSeconds(blinkInterval);

            SetRenderers(true);
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2f;
        }

        SetRenderers(true);
        isInvincible = false;
    }

    private void SetRenderers(bool state)
    {
        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = state;
        }
    }
}