using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Referencia al Animator")]
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    [SerializeField] private float groundedBufferTime = 0.1f;

    [Header("Animación de inicio")]
    // Ajustar en Inspector para que coincida con la duración real del clip FBX.
    // Durante este tiempo puedeMoverse = false.
    [SerializeField] private float duracionLevantarse = 2.5f;

    [Header("Roll")]
    // Ajustar en Inspector. Durante el roll puedeMoverse = false.
    [SerializeField] private float duracionRoll = 0.8f;

    [Header("Efectos de Audio")]
    public AudioSource audioGolpe;
    public AudioSource audioSalto;
    public AudioSource audioCorrer;

    private PlayerMovement _movement;
    private PlayerInputHandler _input;
    private float _airTimer;
    private bool _jumpSoundPlayed;
    private bool _wasGrounded;
    private bool _wasDiving; // frame anterior de IsDiving, para detectar flanco ascendente

    // Flag estático: sobrevive reactivaciones del objeto y entradas/salidas de pausa.
    // Se resetea con ResetLevantarse(), que llama GameManager.RestartGame().
    private static bool _levantarseYaReproducido = false;

    // Hashes cacheados para evitar strings en Update
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityParam = Animator.StringToHash("VerticalVelocity");
    private static readonly int LevantarseParam = Animator.StringToHash("Levantarse");
    private static readonly int DiveParam = Animator.StringToHash("Dive");
    private static readonly int RollParam = Animator.StringToHash("Roll");

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _input = GetComponent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // Si ya se reprodujo antes (pausa, reactivación), no bloquear ni disparar trigger.
        if (_levantarseYaReproducido) return;
        StartCoroutine(SecuenciaLevantarse());
    }

    private IEnumerator SecuenciaLevantarse()
    {
        // Se marca ANTES de esperar: si algo interrumpe la coroutine,
        // no se vuelve a disparar en el próximo Start().
        _levantarseYaReproducido = true;
        _movement.puedeMoverse = false;
        animator.SetTrigger(LevantarseParam);
        yield return new WaitForSeconds(duracionLevantarse);
        _movement.puedeMoverse = true;

        // Forzar salida del estado levantarseChef en el Animator.
        // Si la transición hacia Idle no está configurada con Has Exit Time,
        // este Play garantiza que el Animator no quede congelado en la pose final.
        animator.Play("Idle", 0, 0f);
    }

    // Llamado por GameManager.RestartGame() para que la animación de inicio
    // se reproduzca de nuevo al comenzar una partida nueva.
    public static void ResetLevantarse() => _levantarseYaReproducido = false;

    private void Update()
    {
        if (PlayerHealth.IsDead) return;
        if (animator == null) return;

        bool physicsGrounded = _movement.IsGrounded;

        if (physicsGrounded) _airTimer = 0f;
        else _airTimer += Time.deltaTime;

        bool animatorGrounded = _airTimer < groundedBufferTime;

        animator.SetFloat(SpeedParam, _input.MoveInput.magnitude);
        animator.SetBool(IsGroundedParam, animatorGrounded);

        float vertVel = animatorGrounded ? 0f : _movement.VerticalVelocity;
        animator.SetFloat(VerticalVelocityParam, vertVel);

        HandleDiveAnimation();
        HandleRollAnimation();
        HandleJumpSound();
        HandleRunSound(physicsGrounded);

        _wasGrounded = physicsGrounded;
    }

    // Detecta flanco ascendente de IsDiving para disparar el trigger una sola vez.
    // Si se disparara cada frame que _isDiving es true, el Animator reiniciaría la animación.
    private void HandleDiveAnimation()
    {
        bool isDivingNow = _movement.IsDiving;
        if (isDivingNow && !_wasDiving)
            animator.SetTrigger(DiveParam);
        _wasDiving = isDivingNow;
    }

    // Lee LandedAfterDive y consume el flag inmediatamente para no disparar el roll dos veces.
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
        _movement.StartRollMomentum(); // captura inercia real del dive antes de bloquear
        _movement.puedeMoverse = false;
        animator.SetTrigger(RollParam);
        yield return new WaitForSeconds(duracionRoll);
        _movement.puedeMoverse = true;
    }

    // Llamado por Animation Event en el clip de ataque.
    public void PlayAttackSound()
    {
        if (audioGolpe != null) audioGolpe.Play();
    }

    private void HandleJumpSound()
    {
        float verticalVel = _movement.VerticalVelocity;

        if (verticalVel > 0.1f && !_jumpSoundPlayed)
        {
            if (audioSalto != null) audioSalto.Play();
            _jumpSoundPlayed = true;
        }

        if (_movement.IsGrounded)
            _jumpSoundPlayed = false;
    }

    private void HandleRunSound(bool grounded)
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
