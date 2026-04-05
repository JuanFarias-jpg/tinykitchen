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

    [Header("Audios")]
    public AudioSource Damage;
    public AudioSource Dead;

    private bool isInvincible = false;

    public static bool IsDead = false; 

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
}