using UnityEngine;
using System.Collections;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonView))]
public class PlayerMovementMulti : MonoBehaviour
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

    [Header("Dive")]
    [SerializeField] private float diveSpeed = 18f;
    [SerializeField] private float diveFriction = 8f;
    [SerializeField] private float rollMinSpeed = 4f;
    [SerializeField] private float rollFriction = 12f;
    [SerializeField] private float diveActiveThreshold = 1f;

    private bool _diveUsed;
    private Vector3 _diveDirection;
    private Vector3 _diveVelocity;
    private Vector3 _rollVelocity;

    public bool IsDiving =>
        _diveVelocity.sqrMagnitude >
        diveActiveThreshold * diveActiveThreshold;

    public bool LandedAfterDive { get; private set; }

    private CharacterController _controller;
    private PlayerInputHandler _input;
    private PhotonView pv;

    private float _coyoteTimer;
    private float _verticalVelocity;

    private Coroutine _apagarParticulasCoroutine;

    public bool IsGrounded => _controller.isGrounded;
    public float VerticalVelocity => _verticalVelocity;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();

        // =========================
        // MULTIPLAYER
        // =========================

        if (!pv.IsMine)
        {
            // Desactivar cámara remota
            Camera cam = GetComponentInChildren<Camera>();

            if (cam != null)
                cam.gameObject.SetActive(false);

            // Desactivar AudioListener remoto
            AudioListener listener =
                GetComponentInChildren<AudioListener>();

            if (listener != null)
                listener.enabled = false;

            // Desactivar input remoto
            if (_input != null)
                _input.enabled = false;

            return;
        }
        if (!pv.IsMine)
        {
            // gravedad remota mínima
            if (!_controller.isGrounded)
            {
                _verticalVelocity += gravity * Time.deltaTime;

                Vector3 remoteMove = Vector3.up * _verticalVelocity;

                _controller.Move(remoteMove * Time.deltaTime);
            }

            return;
        }

        // Buscar cámara local automáticamente
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();

            if (cam != null)
                cameraTransform = cam.transform;
        }
    }

    private void Update()
    {
        // SOLO jugador local se mueve
        if (!pv.IsMine)
            return;

        HandleDiveFriction();
        HandleRollMomentum();

        if (!puedeMoverse)
        {
            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 blockedMove = _rollVelocity;
            blockedMove.y = _verticalVelocity;

            if (blockedMove.sqrMagnitude > 0.01f)
                _controller.Move(blockedMove * Time.deltaTime);

            if (particulas != null)
                particulas.SetActive(false);

            return;
        }

        HandleMovement();
        HandleJump();
        HandleDive();
    }

    private void HandleDiveFriction()
    {
        if (_diveVelocity.sqrMagnitude < 0.01f)
            return;

        _diveVelocity = Vector3.MoveTowards(
            _diveVelocity,
            Vector3.zero,
            diveFriction * Time.deltaTime);
    }

    private void HandleRollMomentum()
    {
        if (_rollVelocity.sqrMagnitude < 0.01f)
            return;

        _rollVelocity = Vector3.MoveTowards(
            _rollVelocity,
            Vector3.zero,
            rollFriction * Time.deltaTime);
    }

    public void StartRollMomentum()
    {
        _rollVelocity = _diveVelocity.magnitude > 0.01f
            ? _diveDirection * _diveVelocity.magnitude
            : Vector3.zero;
    }

    private void HandleMovement()
    {
        LandedAfterDive = false;

        if (_controller.isGrounded)
        {
            if (_diveVelocity.magnitude > rollMinSpeed)
                LandedAfterDive = true;

            _diveVelocity = Vector3.zero;
            _diveUsed = false;
        }

        Vector2 input = _input.MoveInput;

        // =========================
        // MOVIMIENTO TIPO ODYSSEY
        // =========================

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            forward * input.y +
            right * input.x;

        // =========================

        Vector3 diveContribution = _diveVelocity;
        diveContribution.y = 0f;

        if (_controller.isGrounded &&
            input.sqrMagnitude > 0.1f)
        {
            if (particulas != null)
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
                _apagarParticulasCoroutine =
                    StartCoroutine(ApagarParticulasConDelay());
        }

        if (_controller.isGrounded &&
            _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
            _coyoteTimer = coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        _verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed =
            playerSpeed.Value *
            (_controller.isGrounded ? 1f : airControlFactor);

        Vector3 finalMovement =
            moveDirection * currentSpeed +
            diveContribution;

        finalMovement.y = _verticalVelocity;

        _controller.Move(finalMovement * Time.deltaTime);

        // =========================
        // ROTACIÓN
        // =========================

        if (IsDiving)
        {
            transform.rotation =
                Quaternion.LookRotation(_diveDirection);
        }
        else if (input.sqrMagnitude > 0.01f)
        {
            Vector3 lookDir = moveDirection.normalized;

            lookDir.y = 0f;

            Quaternion targetRotation =
                Quaternion.LookRotation(lookDir);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (_input.JumpPressed &&
            (_controller.isGrounded || _coyoteTimer > 0f))
        {
            _verticalVelocity = jumpForce.Value;
            _coyoteTimer = 0f;
        }

        _input.ConsumeJump();
    }

    private void HandleDive()
    {
        if (!_controller.isGrounded &&
            !_diveUsed &&
            !IsDiving &&
            _input.DivePressed)
        {
            _diveUsed = true;

            Vector2 moveInput = _input.MoveInput;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 fwd = cameraTransform.forward;
                Vector3 rgt = cameraTransform.right;

                fwd.y = 0f;
                rgt.y = 0f;

                fwd.Normalize();
                rgt.Normalize();

                _diveDirection =
                    (fwd * moveInput.y +
                     rgt * moveInput.x).normalized;
            }
            else
            {
                _diveDirection = transform.forward;
            }

            _diveVelocity =
                _diveDirection * diveSpeed;
        }

        _input.ConsumeDive();
    }

    public void ConsumeRollLanding()
    {
        LandedAfterDive = false;
    }

    IEnumerator ApagarParticulasConDelay()
    {
        yield return new WaitForSeconds(0.2f);

        if (particulas != null)
            particulas.SetActive(false);

        _apagarParticulasCoroutine = null;
    }
}