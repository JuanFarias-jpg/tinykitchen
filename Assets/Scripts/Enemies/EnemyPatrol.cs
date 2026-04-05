using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrullaje Random")]
    public float patrolRadius = 10f;
    public float patrolSpeed = 2f;
    private Vector3 randomTarget;

    [Header("Idle")]
    public float timeBetweenIdle = 4f;
    public float idleDuration = 2f;
    private float idleTimer;

    [Header("Audio")]
    public AudioSource audioIdle;
    public AudioSource audioRun;
    public AudioSource audioAttack;

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

    [Header("Ataque")]
    public float attackCooldown = 1f;
    private float lastAttackTime;
    private bool canAttack = true;

    [Header("Anti-Bug")]
    public float stuckCheckTime = 1f;
    public float minMoveDistance = 0.2f;
    private Vector3 lastPosition;
    private float stuckTimer;

    public float wallCheckDistance = 1f;

    private Transform player;
    private Animator animator;
    private CharacterController controller;

    private enum State { Patrol, Idle, Chase, Attack }
    private State currentState;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        currentState = State.Patrol;
        randomTarget = GetRandomPoint();
        lastPosition = transform.position;
    }

    private void Update()
    {
        ApplyGravity();
        CheckIfStuck();

        float distance = Vector3.Distance(transform.position, player.position);

        idleTimer += Time.deltaTime;

        HandleAudio();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (distance < detectionRange)
                    currentState = State.Chase;

                if (idleTimer >= timeBetweenIdle)
                {
                    idleTimer = 0;
                    currentState = State.Idle;
                    Invoke(nameof(ExitIdle), idleDuration);
                }
                break;

            case State.Idle:
                animator.SetFloat("Speed", 0f);

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

    void HandleAudio()
    {
        // Idle
        if (currentState == State.Idle)
        {
            if (audioIdle != null && !audioIdle.isPlaying)
                audioIdle.Play();
        }
        else
        {
            if (audioIdle != null && audioIdle.isPlaying)
                audioIdle.Stop();
        }

        // Run
        if (currentState == State.Patrol || currentState == State.Chase)
        {
            if (audioRun != null && !audioRun.isPlaying)
                audioRun.Play();
        }
        else
        {
            if (audioRun != null && audioRun.isPlaying)
                audioRun.Stop();
        }
    }

    void Patrol()
    {
        MoveTo(randomTarget, patrolSpeed);

        if (Vector3.Distance(transform.position, randomTarget) < 0.5f)
        {
            randomTarget = GetRandomPoint();
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

            lastAttackTime = Time.time;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }


    public void DealDamage()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (OnPlayerDamaged != null)
                OnPlayerDamaged.Raise();
        }
    }


    public void PlayAttackSound()
    {
        if (audioAttack != null)
            audioAttack.Play();
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, wallCheckDistance))
        {
            if (currentState == State.Patrol)
            {
                randomTarget = GetRandomPoint();
                return;
            }
        }

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

    void CheckIfStuck()
    {
        stuckTimer += Time.deltaTime;

        if (stuckTimer >= stuckCheckTime)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved < minMoveDistance && currentState == State.Patrol)
            {
                randomTarget = GetRandomPoint();
            }

            lastPosition = transform.position;
            stuckTimer = 0f;
        }
    }

    Vector3 GetRandomPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position;
    }

    void ExitIdle()
    {
        if (currentState == State.Idle)
        {
            randomTarget = GetRandomPoint();
            currentState = State.Patrol;
        }
    }
}