using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputHandler input;

    [Header("Ataque")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] public int damage = 1;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;

    private float lastAttackTime;
    private bool isAttacking;

    private static readonly int AttackParam = Animator.StringToHash("Attack");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (input == null)
            input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleAttack();
    }

    void HandleAttack()
    {
        if (!input.AttackPressed) return;

        
        if (Time.time < lastAttackTime + attackCooldown || isAttacking)
        {
            input.ConsumeAttack();
            return;
        }

        isAttacking = true;

        animator.ResetTrigger(AttackParam);
        animator.SetTrigger(AttackParam);

        lastAttackTime = Time.time;

        input.ConsumeAttack();

        
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

  
    public void DoAttack()
    {
        Collider[] enemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider enemy in enemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}