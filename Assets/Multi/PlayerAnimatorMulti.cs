using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerAnimatorMulti : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Grounded")]
    [SerializeField] private float groundedBufferTime = 0.1f;

    [Header("Levantarse")]
    [SerializeField] private float duracionLevantarse = 2.5f;

    [Header("Roll")]
    [SerializeField] private float duracionRoll = 0.8f;

    [Header("Audio")]
    public AudioSource audioGolpe;
    public AudioSource audioSalto;
    public AudioSource audioCorrer;

    private PlayerMovementMulti _movement;
    private PlayerInputHandler _input;

    private PhotonView pv;

    private float _airTimer;

    private bool _jumpSoundPlayed;
    private bool _wasDiving;

    private static bool _levantarseYaReproducido = false;

    private static readonly int SpeedParam =
        Animator.StringToHash("Speed");

    private static readonly int IsGroundedParam =
        Animator.StringToHash("IsGrounded");

    private static readonly int VerticalVelocityParam =
        Animator.StringToHash("VerticalVelocity");

    private static readonly int LevantarseParam =
        Animator.StringToHash("Levantarse");

    private static readonly int DiveParam =
        Animator.StringToHash("Dive");

    private static readonly int RollParam =
        Animator.StringToHash("Roll");

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        _movement = GetComponent<PlayerMovementMulti>();
        _input = GetComponent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // Remotos NO controlan animator
        if (!pv.IsMine)
            return;

        if (_levantarseYaReproducido)
            return;

        StartCoroutine(SecuenciaLevantarse());
    }

    private IEnumerator SecuenciaLevantarse()
    {
        _levantarseYaReproducido = true;

        _movement.puedeMoverse = false;

        animator.SetTrigger(LevantarseParam);

        yield return new WaitForSeconds(duracionLevantarse);

        _movement.puedeMoverse = true;

        animator.Play("Idle", 0, 0f);
    }

    public static void ResetLevantarse()
    {
        _levantarseYaReproducido = false;
    }

    private void Update()
    {
        // SOLO local controla animaciones
        if (!pv.IsMine)
            return;

        if (PlayerHealth.IsDead)
            return;

        if (animator == null)
            return;

        bool physicsGrounded =
            _movement.IsGrounded;

        if (physicsGrounded)
            _airTimer = 0f;
        else
            _airTimer += Time.deltaTime;

        bool animatorGrounded =
            _airTimer < groundedBufferTime;

        animator.SetFloat(
            SpeedParam,
            _input.MoveInput.magnitude);

        animator.SetBool(
            IsGroundedParam,
            animatorGrounded);

        float vertVel =
            animatorGrounded
            ? 0f
            : _movement.VerticalVelocity;

        animator.SetFloat(
            VerticalVelocityParam,
            vertVel);

        HandleDiveAnimation();
        HandleRollAnimation();
        HandleJumpSound();
        HandleRunSound(physicsGrounded);
    }

    private void HandleDiveAnimation()
    {
        bool isDivingNow =
            _movement.IsDiving;

        if (isDivingNow && !_wasDiving)
            animator.SetTrigger(DiveParam);

        _wasDiving = isDivingNow;
    }

    private void HandleRollAnimation()
    {
        if (_movement.LandedAfterDive)
        {
            _movement.ConsumeRollLanding();

            StartCoroutine(SecuenciaRoll());
        }
    }

    private IEnumerator SecuenciaRoll()
    {
        _movement.StartRollMomentum();

        _movement.puedeMoverse = false;

        animator.SetTrigger(RollParam);

        yield return new WaitForSeconds(duracionRoll);

        _movement.puedeMoverse = true;
    }

    public void PlayAttackSound()
    {
        if (audioGolpe != null)
            audioGolpe.Play();
    }

    private void HandleJumpSound()
    {
        float verticalVel =
            _movement.VerticalVelocity;

        if (verticalVel > 0.1f &&
            !_jumpSoundPlayed)
        {
            if (audioSalto != null)
                audioSalto.Play();

            _jumpSoundPlayed = true;
        }

        if (_movement.IsGrounded)
            _jumpSoundPlayed = false;
    }

    private void HandleRunSound(bool grounded)
    {
        if (grounded &&
            _input.MoveInput.magnitude > 0.1f)
        {
            if (audioCorrer != null &&
                !audioCorrer.isPlaying)
            {
                audioCorrer.Play();
            }
        }
        else
        {
            if (audioCorrer != null &&
                audioCorrer.isPlaying)
            {
                audioCorrer.Stop();
            }
        }
    }
}