using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonView))]
public class EnemyPatrolMulti : MonoBehaviourPun, IPunObservable
{
    [Header("Patrullaje Random")]
    public float patrolRadius = 10f;
    public float patrolSpeed = 2f;

    [Header("Idle")]
    public float timeBetweenIdle = 4f;
    public float idleDuration = 2f;

    [Header("Audio")]
    public AudioSource audioIdle;
    public AudioSource audioRun;
    public AudioSource audioAttack;

    [Header("Deteccion")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    [Header("Persecucion")]
    public float chaseSpeed = 4f;

    [Header("Gravedad")]
    public float gravity = -20f;

    [Header("Ataque")]
    public float attackCooldown = 1f;
    public int attackDamage = 10;         

    [Header("Anti-Bug")]
    public float stuckCheckTime = 1f;
    public float minMoveDistance = 0.2f;
    public float wallCheckDistance = 1f;

    private Vector3 randomTarget;
    private float idleTimer;
    private float verticalVelocity;
    private float lastAttackTime;
    private bool canAttack = true;
    private Vector3 lastPosition;
    private float stuckTimer;

    private Transform targetPlayer;
    private Animator animator;
    private CharacterController controller;
    private PhotonView pv;

    private enum State { Patrol, Idle, Chase, Attack }
    private State currentState = State.Patrol;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // Solo el Master Client controla la IA del enemigo
        if (!pv.IsMine)
        {
            enabled = false;
            return;
        }

        lastPosition = transform.position;
        randomTarget = GetRandomPoint();
        currentState = State.Patrol;
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        ApplyGravity();
        CheckIfStuck();
        FindClosestPlayer();

        idleTimer += Time.deltaTime;
        HandleAudio();

        float distanceToPlayer = targetPlayer != null ?
            Vector3.Distance(transform.position, targetPlayer.position) : 999f;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (distanceToPlayer < detectionRange)
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
                if (distanceToPlayer < detectionRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                Chase();
                if (distanceToPlayer < attackRange)
                    currentState = State.Attack;
                else if (distanceToPlayer > detectionRange + 3f)
                    currentState = State.Patrol;
                break;

            case State.Attack:
                Attack();
                if (distanceToPlayer > attackRange)
                    currentState = State.Chase;
                break;
        }
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = p.transform;
            }
        }

        targetPlayer = closest;
    }

    void HandleAudio()
    {
        // Idle
        if (currentState == State.Idle)
        {
            if (audioIdle != null && !audioIdle.isPlaying) audioIdle.Play();
        }
        else if (audioIdle != null && audioIdle.isPlaying)
            audioIdle.Stop();

        // Run
        if (currentState == State.Patrol || currentState == State.Chase)
        {
            if (audioRun != null && !audioRun.isPlaying) audioRun.Play();
        }
        else if (audioRun != null && audioRun.isPlaying)
            audioRun.Stop();
    }

    void Patrol()
    {
        MoveTo(randomTarget, patrolSpeed);
        if (Vector3.Distance(transform.position, randomTarget) < 0.5f)
            randomTarget = GetRandomPoint();

        animator.SetFloat("Speed", patrolSpeed);
    }

    void Chase()
    {
        if (targetPlayer == null) return;
        MoveTo(targetPlayer.position, chaseSpeed);
        animator.SetFloat("Speed", chaseSpeed);
    }

    void Attack()
    {
        if (targetPlayer == null) return;

        // Mirar al jugador
        Vector3 direction = (targetPlayer.position - transform.position);
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 12f * Time.deltaTime);
        }

        animator.SetFloat("Speed", 0f);

        if (canAttack && Time.time >= lastAttackTime + attackCooldown)
        {
            canAttack = false;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            DealDamage();

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    public void DealDamage()
    {
        if (targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);
        if (distance > attackRange) return;

        PhotonView playerPV = targetPlayer.GetComponent<PhotonView>();
        if (playerPV != null)
        {
            // Llamada correcta al PlayerHealth
            playerPV.RPC("TakeDamage", playerPV.Owner, attackDamage);
        }
    }

    public void PlayAttackSound()
    {
        if (audioAttack != null)
            audioAttack.Play();
    }

    void ResetAttack() => canAttack = true;

    void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;

        // Evitar paredes
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, wallCheckDistance))
        {
            if (currentState == State.Patrol)
                randomTarget = GetRandomPoint();
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
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    void CheckIfStuck()
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer >= stuckCheckTime)
        {
            if (Vector3.Distance(transform.position, lastPosition) < minMoveDistance && currentState == State.Patrol)
                randomTarget = GetRandomPoint();

            lastPosition = transform.position;
            stuckTimer = 0f;
        }
    }

    Vector3 GetRandomPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
            randomDir += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, patrolRadius, NavMesh.AllAreas))
                return hit.position;
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

    // ====================== SINCRONIZACIÓN PHOTON ======================
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext((int)currentState);
            stream.SendNext(animator.GetFloat("Speed"));
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            currentState = (State)(int)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
        }
    }
}