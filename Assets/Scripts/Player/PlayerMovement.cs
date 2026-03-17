using UnityEngine;

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

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;
    [Header("Rotación")]    [SerializeField] private float rotationSpeed = 10f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController _controller;
    private PlayerInputHandler _input;
    private float _coyoteTimer;
    //velocidad vertical actual (gravedad + salto)
    private float _verticalVelocity;
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
        HandleMovement();
        HandleJump();
    }
    private void HandleMovement()
    {
        Vector2 input = _input.MoveInput;

        //obtener los ejes de la cámara y aplanarlos
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

    
        Vector3 moveDirection = forward * input.y + right * input.x;

        //aplicar factor de control aéreo si no está en el piso
        float speedMultiplier = _controller.isGrounded ? 1f : airControlFactor;


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


        Vector3 finalMovement = moveDirection * playerSpeed.Value * speedMultiplier;
        finalMovement.y = _verticalVelocity;

        _controller.Move(finalMovement * Time.deltaTime);

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
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
}