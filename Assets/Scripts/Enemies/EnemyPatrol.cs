using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrullaje")]
    public Transform[] waypoints;
    public float patrolSpeed = 2f;

    [Header("Detección")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    [Header("Persecución")]
    public float chaseSpeed = 4f;

    [Header("Gravedad")]
    public float gravity = -20f;
    private float verticalVelocity;

    [Header("Eventos")]
    public GameEvent OnPlayerDamaged;

    private int currentWaypoint = 0;
    private Transform player;
    private Animator animator;
    private CharacterController controller;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    private float attackCooldown = 1f;
    private float lastAttackTime;
    private bool canAttack = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        currentState = State.Patrol;
    }

    private void Update()
    {
        ApplyGravity();

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (distance < detectionRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                Chase();

                if (distance < attackRange)
                    currentState = State.Attack;
                else if (distance > detectionRange)
                    currentState = State.Patrol;
                break;

            case State.Attack:
                Attack();

                if (distance > attackRange)
                    currentState = State.Chase;
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        MoveTo(target.position, patrolSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        animator.SetFloat("Speed", patrolSpeed);
    }

    void Chase()
    {
        MoveTo(player.position, chaseSpeed);
        animator.SetFloat("Speed", chaseSpeed);
    }

    void Attack()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        animator.SetFloat("Speed", 0f);

        if (canAttack && Time.time >= lastAttackTime + attackCooldown)
        {
            canAttack = false;

            animator.SetTrigger("Attack");

            if (OnPlayerDamaged != null)
                OnPlayerDamaged.Raise();

            lastAttackTime = Time.time;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }
    void ResetAttack()
    {
        canAttack = true;
    }

    void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;

        Vector3 move = direction * speed;

        
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}