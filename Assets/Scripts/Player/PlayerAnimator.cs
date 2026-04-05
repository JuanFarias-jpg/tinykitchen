using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Referencia al Animator")]
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    [SerializeField] private float groundedBufferTime = 0.1f;

    [Header("Efectos de Audio")]
    public AudioSource audioGolpe;
    public AudioSource audioSalto;
    public AudioSource audioCorrer;

    private PlayerMovement _movement;
    private PlayerInputHandler _input;
    private float _airTimer;

    private bool jumpSoundPlayed;
    private bool wasGrounded;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityParam = Animator.StringToHash("VerticalVelocity");

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _input = GetComponent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (PlayerHealth.IsDead) return;
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

        HandleJumpSound();
        HandleRunSound(physicsGrounded);

        wasGrounded = physicsGrounded;
    }

    
    public void PlayAttackSound()
    {
        if (audioGolpe != null)
            audioGolpe.Play();
    }

    void HandleJumpSound()
    {
        float verticalVel = _movement.VerticalVelocity;

        if (verticalVel > 0.1f && !jumpSoundPlayed)
        {
            if (audioSalto != null)
                audioSalto.Play();

            jumpSoundPlayed = true;
        }

        if (_movement.IsGrounded)
        {
            jumpSoundPlayed = false;
        }
    }

    void HandleRunSound(bool grounded)
    {
        if (grounded && _input.MoveInput.magnitude > 0.1f)
        {
            if (audioCorrer != null && !audioCorrer.isPlaying)
                audioCorrer.Play();
        }
        else
        {
            if (audioCorrer != null && audioCorrer.isPlaying)
                audioCorrer.Stop();
        }
    }
}