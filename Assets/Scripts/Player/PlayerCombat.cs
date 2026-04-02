using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputHandler input;

    [Header("Ataque")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;

    private float lastAttackTime;

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

        if (Time.time < lastAttackTime + attackCooldown) return;

      
        animator.SetTrigger(AttackParam);

        
        DoAttack();

        lastAttackTime = Time.time;

        input.ConsumeAttack();
    }

    void DoAttack()
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

    // Para ver el rango en la escena
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}