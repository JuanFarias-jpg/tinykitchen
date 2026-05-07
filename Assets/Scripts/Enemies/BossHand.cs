using UnityEngine;
using System.Collections;

public class BossHand : MonoBehaviour
{
    [Header("Boss")]
    public BossController bossController;

    [Header("Vida Mano")]
    public int totalFingers = 5;
    private int burnedFingers = 0;

    [Header("Daño Boss")]
    public int damageToBoss = 50;

    [Header("Movimiento")]
    public float sideDistance = 8f;
    public float downDistance = 20f;

    public float sideMoveSpeed = 4f;
    public float moveDownSpeed = 8f;
    public float moveUpSpeed = 10f;

    [Header("Timing")]
    public float attackCooldown = 2f;
    public float attackDuration = 5f;

    [Header("Collider de Daño")]
    public Collider damageCollider;

    [Header("Animación")]
    public Animator animator;

    private Vector3 idlePosition;
    private Vector3 currentTopPosition;
    private Vector3 attackPosition;

    private bool isDead = false;

    private static readonly int AttackParam = Animator.StringToHash("Attack");
    private static readonly int HurtParam = Animator.StringToHash("Hurt");
    private static readonly int DieParam = Animator.StringToHash("Die");

    private void Start()
    {
        idlePosition = transform.position;

        DisableDamageCollider();

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (!isDead)
        {
            DisableDamageCollider();

            yield return StartCoroutine(MoveSideways());
            yield return StartCoroutine(MoveDownIdle());
            yield return StartCoroutine(MoveUpIdle());

            yield return StartCoroutine(PlayAttackAnimation());

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    IEnumerator MoveSideways()
    {
        float randomX = Random.Range(-sideDistance, sideDistance);

        Vector3 target = new Vector3(
            idlePosition.x + randomX,
            transform.position.y,
            transform.position.z
        );

        while (Mathf.Abs(transform.position.x - target.x) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                sideMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;

        currentTopPosition = target;

        attackPosition = new Vector3(
            currentTopPosition.x,
            idlePosition.y - downDistance,
            currentTopPosition.z
        );
    }

    IEnumerator MoveDownIdle()
    {
        while (Vector3.Distance(transform.position, attackPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                attackPosition,
                moveDownSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    IEnumerator MoveUpIdle()
    {
        while (Vector3.Distance(transform.position, currentTopPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTopPosition,
                moveUpSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    IEnumerator PlayAttackAnimation()
    {
        EnableDamageCollider();

        animator.SetTrigger(AttackParam);

        yield return new WaitForSeconds(attackDuration);

        DisableDamageCollider();
    }

    void EnableDamageCollider()
    {
       
            damageCollider.enabled = true;
    }

    void DisableDamageCollider()
    {
        
            damageCollider.enabled = false;
    }

    public void BurnFinger()
    {
        if (isDead) return;

        burnedFingers++;

        animator.SetTrigger(HurtParam);

        if (burnedFingers >= totalFingers)
            Die();
    }

    void Die()
    {
        isDead = true;

        StopAllCoroutines();

        DisableDamageCollider();

        animator.SetTrigger(DieParam);

        bossController?.DamageBoss(damageToBoss);

        Destroy(gameObject, 3f);
    }
}