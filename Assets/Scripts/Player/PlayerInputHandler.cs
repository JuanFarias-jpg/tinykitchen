using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    //input actions asset
    private PlayerControls _controls;

    //datos de entrada para otros scripts
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Gameplay.Enable();

        //suscribirse a los eventos de cada acción
        _controls.Gameplay.Jump.performed += OnJump;
        _controls.Gameplay.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        //desuscribirse a los eventos
        _controls.Gameplay.Jump.performed -= OnJump;
        _controls.Gameplay.Attack.performed -= OnAttack;

        _controls.Gameplay.Disable();
    }

    private void Update()
    {
        //move se lee cada frame porque es un valor continuo 
        MoveInput = _controls.Gameplay.Move.ReadValue<Vector2>();

    }
    public void ConsumeJump()
    {
        JumpPressed = false;
    }

    public void ConsumeAttack()
    {
        AttackPressed = false;
    }

    //callbacks del Input System
    private void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed = true;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        AttackPressed = true;
    }
}