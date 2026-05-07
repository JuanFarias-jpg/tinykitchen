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
    public bool DivePressed { get; private set; }

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
        _controls.Gameplay.Dive.performed += OnDive;
    }

    private void OnDisable()
    {
        //desuscribirse a los eventos
        _controls.Gameplay.Jump.performed -= OnJump;
        _controls.Gameplay.Attack.performed -= OnAttack;
        _controls.Gameplay.Dive.performed += OnDive;

        _controls.Gameplay.Disable();
    }

    private void Update()
    {
        //move se lee cada frame porque es un valor continuo 
        MoveInput = _controls.Gameplay.Move.ReadValue<Vector2>();

    }
    public void ConsumeJump()   => JumpPressed   = false;
    public void ConsumeAttack() => AttackPressed = false;

    // ConsumeDive: llamado por PlayerMovement en HandleDive() justo después de
    // leer el flag, para que no se dispare el dive dos veces en el mismo salto.
    public void ConsumeDive()   => DivePressed   = false;

    private void OnJump(InputAction.CallbackContext _)   => JumpPressed   = true;
    private void OnAttack(InputAction.CallbackContext _) => AttackPressed = true;
    private void OnDive(InputAction.CallbackContext _)   => DivePressed   = true;
}