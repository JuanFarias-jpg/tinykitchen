using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 6.5f;
    public float sprintSpeed = 9f;
    public float acceleration = 18f;
    public float airControl = 0.55f;
    public float turnSmoothTime = 0.08f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -24f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.12f;

    private CharacterController cc;
    private Vector3 velocity;
    private Vector3 currentMove;
    private float turnSmoothVelocity;

    private float coyoteTimer;
    private float jumpBufferTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleTimers();
        HandleMovement();
        HandleJumpandGravity();
    }

    void HandleTimers()
    {
        if (IsGrounded())
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(x, 0f, z).normalized;

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        Vector3 desiredMove = Vector3.zero;

        if (inputDir.sqrMagnitude > 0.001f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            desiredMove = (camForward * inputDir.z + camRight * inputDir.x).normalized * targetSpeed;

            float targetAngle = Mathf.Atan2(desiredMove.x, desiredMove.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        float control = IsGrounded() ? 1f : airControl;
        Vector3 targetMove = Vector3.Lerp(currentMove, desiredMove, control * acceleration * Time.deltaTime);
        currentMove = targetMove;

        cc.Move(currentMove * Time.deltaTime);
    }

    void HandleJumpandGravity()
    {
        if (IsGrounded() && velocity.y < 0f)
            velocity.y = -2f;

        bool canJump = coyoteTimer > 0f;
        bool wantsJump = jumpBufferTimer > 0f;

        if (canJump && wantsJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return cc.isGrounded;
    }
}
