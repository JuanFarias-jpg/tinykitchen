using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Referencia al Animator")]
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    [SerializeField] private float groundedBufferTime = 0.1f;

    private PlayerMovement _movement;
    private PlayerInputHandler _input;
    private float _airTimer;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityParam = Animator.StringToHash("VerticalVelocity");
    private static readonly int AttackParam = Animator.StringToHash("Attack");
    private static readonly int DieParam = Animator.StringToHash("Die");

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _input = GetComponent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (animator == null) return;

        bool physicsGrounded = _movement.IsGrounded;

        if (physicsGrounded)
            _airTimer = 0f;
        else
            _airTimer += Time.deltaTime;

        bool animatorGrounded = _airTimer < groundedBufferTime;

        animator.SetFloat(SpeedParam, _input.MoveInput.magnitude);
        animator.SetBool(IsGroundedParam, animatorGrounded);

        float vertVel = animatorGrounded ? 0f : _movement.VerticalVelocity;
        animator.SetFloat(VerticalVelocityParam, vertVel);

        
        if (_input.AttackPressed)
        {
            animator.SetTrigger(AttackParam);
            _input.ConsumeAttack();
        }
    }


    public void PlayDeath()
    {
        animator.SetTrigger(DieParam);
    }
}