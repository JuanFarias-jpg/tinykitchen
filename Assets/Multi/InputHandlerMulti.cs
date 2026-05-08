using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerInputHandlerMulti : MonoBehaviour
{
    // Input Actions
    private PlayerControls _controls;

    // Referencia Photon
    private PhotonView pv;

    // Datos de entrada
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool DivePressed { get; private set; }

    [Header("Dive Cooldown")]
    public float diveCooldown = 4f;

    private float nextDiveTime;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        // SOLO jugador local usa input
        if (pv != null && !pv.IsMine)
            return;

        _controls.Gameplay.Enable();

        _controls.Gameplay.Jump.performed += OnJump;
        _controls.Gameplay.Attack.performed += OnAttack;
        _controls.Gameplay.Dive.performed += OnDive;
    }

    private void OnDisable()
    {
        // SOLO jugador local
        if (pv != null && !pv.IsMine)
            return;

        _controls.Gameplay.Jump.performed -= OnJump;
        _controls.Gameplay.Attack.performed -= OnAttack;
        _controls.Gameplay.Dive.performed -= OnDive;

        _controls.Gameplay.Disable();
    }

    private void Update()
    {
        // SOLO jugador local
        if (pv != null && !pv.IsMine)
            return;

        // Movimiento continuo
        MoveInput =
            _controls.Gameplay.Move.ReadValue<Vector2>();
    }

    // =========================
    // CONSUME
    // =========================

    public void ConsumeJump()
    {
        JumpPressed = false;
    }

    public void ConsumeAttack()
    {
        AttackPressed = false;
    }

    public void ConsumeDive()
    {
        DivePressed = false;
    }

    // =========================
    // INPUT EVENTS
    // =========================

    private void OnJump(InputAction.CallbackContext _)
    {
        JumpPressed = true;
    }

    private void OnAttack(InputAction.CallbackContext _)
    {
        AttackPressed = true;
    }

    private void OnDive(InputAction.CallbackContext _)
    {
        // Cooldown
        if (Time.time < nextDiveTime)
            return;

        DivePressed = true;

        nextDiveTime =
            Time.time + diveCooldown;
    }
}