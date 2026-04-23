using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Variables")]
    public IntVariable playerHP;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    public int damageAmount = 1;
    public float invincibilityTime = 1f;

    [Header("Parpadeo")]
    public float blinkInterval = 0.2f;
    private Renderer[] renderers;

    [Header("Audios")]
    public AudioSource Damage;
    public AudioSource Dead;

    private bool isInvincible = false;
    public static bool IsDead = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void TakeDamage()
    {
        if (isInvincible || IsDead) return;

        playerHP.Value -= damageAmount;

        Damage.Play();

        if (playerHP.Value <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFrames());
    }

    void Die()
    {
        IsDead = true;
        Dead.Play();

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

        Destroy(gameObject, 3.5f);
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            // Apagar
            SetRenderers(false);
            yield return new WaitForSeconds(blinkInterval);

            // Encender
            SetRenderers(true);
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2;
        }

        
        SetRenderers(true);

        isInvincible = false;
    }

    void SetRenderers(bool state)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = state;
        }
    }
}