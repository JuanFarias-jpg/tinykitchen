using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias de ScriptableObject Variables")]
    [SerializeField] private FloatVariable playerSpeed;
    [SerializeField] private FloatVariable jumpForce;

    [Header("Gravedad")]
    [SerializeField] private float gravity = -20f;

    [Header("Control aéreo")]
    [Range(0f, 1f)]
    [SerializeField] private float airControlFactor = 0.5f;

    [Header("Sistema Particulas")]
    public GameObject particulas;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;

    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    [Header("Control de movimiento")]
    public bool puedeMoverse = true;

    // ── DIVE ──────────────────────────────────────────────────────────────
    [Header("Dive")]
    [SerializeField] private float diveSpeed = 18f;

    // Qué tan rápido se frena la inercia del dive en el aire (unidades/s²).
    [SerializeField] private float diveFriction = 8f;

    // Velocidad mínima al aterrizar para que se dispare el roll.
    // Si el jugador llega más lento que esto, aterriza normal sin roll.
    [SerializeField] private float rollMinSpeed = 4f;

    // Qué tan rápido se frena la inercia durante el roll (unidades/s²).
    [SerializeField] private float rollFriction = 12f;

    // Umbral para considerar que _diveVelocity sigue activa (IsDiving).
    [SerializeField] private float diveActiveThreshold = 1f;

    private bool _diveUsed;
    private Vector3 _diveDirection;
    private Vector3 _diveVelocity;  // velocidad horizontal del dive, se frena con diveFriction
    private Vector3 _rollVelocity;  // inercia capturada al aterrizar, se frena con rollFriction

    // IsDiving: true mientras _diveVelocity supera el umbral.
    // PlayerAnimator detecta el flanco ascendente para disparar el trigger Dive.
    public bool IsDiving => _diveVelocity.sqrMagnitude > diveActiveThreshold * diveActiveThreshold;

    // LandedAfterDive: PlayerAnimator lo lee cada Update para disparar el trigger Roll.
    // Se limpia con ConsumeRollLanding() después de leerlo.
    public bool LandedAfterDive { get; private set; }
    // ──────────────────────────────────────────────────────────────────────

    private CharacterController _controller;
    private PlayerInputHandler _input;
    private float _coyoteTimer;
    private float _verticalVelocity;
    private Coroutine _apagarParticulasCoroutine;

    public bool IsGrounded => _controller.isGrounded;
    public float VerticalVelocity => _verticalVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();

        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }
    }

    private void Update()
    {
        // El freno del dive y el momentum del roll corren siempre,
        // independientemente de puedeMoverse.
        HandleDiveFriction();
        HandleRollMomentum();

        if (!puedeMoverse)
        {
            // Gravedad sigue aplicando durante animaciones bloqueantes.
            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 blockedMove = _rollVelocity;
            blockedMove.y = _verticalVelocity;
            if (blockedMove.sqrMagnitude > 0.01f)
                _controller.Move(blockedMove * Time.deltaTime);

            if (particulas != null) particulas.SetActive(false);
            return;
        }

        HandleMovement();
        HandleJump();
        HandleDive();
    }

    // Frena _diveVelocity en el aire cada frame hasta llegar a cero.
    private void HandleDiveFriction()
    {
        if (_diveVelocity.sqrMagnitude < 0.01f) return;
        _diveVelocity = Vector3.MoveTowards(_diveVelocity, Vector3.zero,
                                             diveFriction * Time.deltaTime);
    }

    // Frena _rollVelocity durante el roll hasta llegar a cero.
    private void HandleRollMomentum()
    {
        if (_rollVelocity.sqrMagnitude < 0.01f) return;
        _rollVelocity = Vector3.MoveTowards(_rollVelocity, Vector3.zero,
                                             rollFriction * Time.deltaTime);
    }

    // Llamado por PlayerAnimator al inicio de SecuenciaRoll().
    // Captura la velocidad real del dive en el momento del landing.
    public void StartRollMomentum()
    {
        _rollVelocity = _diveVelocity.magnitude > 0.01f
            ? _diveDirection * _diveVelocity.magnitude
            : Vector3.zero;
    }

    private void HandleMovement()
    {
        // Detectar landing tras dive — se evalúa al inicio del frame
        LandedAfterDive = false;

        if (_controller.isGrounded)
        {
            // Solo dispara roll si la velocidad de llegada supera rollMinSpeed
            if (_diveVelocity.magnitude > rollMinSpeed)
                LandedAfterDive = true;

            _diveVelocity = Vector3.zero;
            _diveUsed = false;
        }

        Vector2 input = _input.MoveInput;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; forward.Normalize();
        right.y = 0f; right.Normalize();

        Vector3 moveDirection = forward * input.y + right * input.x;

        // La velocidad del dive se suma al movimiento normal.
        // Esto permite corregir levemente la dirección sin cancelar el impulso.
        Vector3 diveContribution = _diveVelocity;
        diveContribution.y = 0f;

        // Partículas
        if (_controller.isGrounded && input.sqrMagnitude > 0.1f)
        {
            particulas.SetActive(true);
            if (_apagarParticulasCoroutine != null)
            {
                StopCoroutine(_apagarParticulasCoroutine);
                _apagarParticulasCoroutine = null;
            }
        }
        else
        {
            if (_apagarParticulasCoroutine == null)
                _apagarParticulasCoroutine = StartCoroutine(ApagarParticulasConDelay());
        }

        // Gravedad y Coyote Time
        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
            _coyoteTimer = coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        _verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed = playerSpeed.Value * (_controller.isGrounded ? 1f : airControlFactor);

        Vector3 finalMovement = moveDirection * currentSpeed + diveContribution;
        finalMovement.y = _verticalVelocity;
        _controller.Move(finalMovement * Time.deltaTime);

        // Rotación — hacia la dirección del dive si está activo, normal si no
        if (IsDiving)
            transform.rotation = Quaternion.LookRotation(_diveDirection);
        else if (input.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                  rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (_input.JumpPressed && (_controller.isGrounded || _coyoteTimer > 0f))
        {
            _verticalVelocity = jumpForce.Value;
            _coyoteTimer = 0f;
        }
        _input.ConsumeJump();
    }

    private void HandleDive()
    {
        // Condiciones: en el aire, dive no usado, _diveVelocity inactiva
        if (!_controller.isGrounded && !_diveUsed && !IsDiving && _input.DivePressed)
        {
            _diveUsed = true;

            // Congelar dirección al momento del dive
            Vector2 moveInput = _input.MoveInput;
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 fwd = cameraTransform.forward; fwd.y = 0f; fwd.Normalize();
                Vector3 rgt = cameraTransform.right; rgt.y = 0f; rgt.Normalize();
                _diveDirection = (fwd * moveInput.y + rgt * moveInput.x).normalized;
            }
            else
            {
                _diveDirection = transform.forward;
            }

            _diveVelocity = _diveDirection * diveSpeed;
        }
        _input.ConsumeDive();
    }

    public void ConsumeRollLanding() => LandedAfterDive = false;

    IEnumerator ApagarParticulasConDelay()
    {
        yield return new WaitForSeconds(0.2f);
        if (particulas != null) particulas.SetActive(false);
        _apagarParticulasCoroutine = null;
    }
}