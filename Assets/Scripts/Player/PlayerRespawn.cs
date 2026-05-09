using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Void")]
    [SerializeField] private float voidYLevel = -20f;

    [Header("Daño")]
    [SerializeField] private int damage = 1;

    [Header("Tiempo para guardar posición segura")]
    [SerializeField] private float savePositionInterval = 3f;
    public GameEvent OnPlayerDamaged;
    private CharacterController controller;
    private PlayerHealth playerHealth;

    private Vector3 safePosition;
    private float timer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();

        safePosition = transform.position;
    }

    private void Update()
    {
        // Guardar posición segura cada ciertos segundos
        timer += Time.deltaTime;

        if (timer >= savePositionInterval && controller.isGrounded)
        {
            safePosition = transform.position;
            timer = 0f;
        }

        // Cayó al vacío
        if (transform.position.y < voidYLevel)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Quitar vida
        if (playerHealth != null)
        {
            OnPlayerDamaged.Raise();
        }

        // Reiniciar velocidad si tienes PlayerMovement
        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.puedeMoverse = false;
        }

        // Desactivar controller para moverlo
        controller.enabled = false;

        transform.position = safePosition;

        controller.enabled = true;

        if (movement != null)
        {
            movement.puedeMoverse = true;
        }
    }
}