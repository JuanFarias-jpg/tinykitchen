using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
    private CharacterController controller;
    private MovingPlatform currentPlatform;

    // Distancia del raycast hacia abajo para detectar si el jugador sigue sobre la plataforma.
    // Debe ser mayor que la mitad de la altura del CharacterController.
    [SerializeField] private float groundCheckDistance = 1.5f;

    // Ángulo mínimo de la normal para considerar que el jugador está ENCIMA (no al lado).
    // 0.7f ≈ 45 grados. 0.85f es más estricto (≈ 32 grados), recomendado para esquinas.
    [SerializeField] private float minNormalY = 0.85f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // OnControllerColliderHit se llama cada vez que el CharacterController toca algo.
    // Solo asignamos currentPlatform cuando la colisión viene de arriba (el jugador está encima),
    // no cuando choca de lado. El filtro por normal.y lo garantiza.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();

        if (platform != null)
        {
            // hit.normal.y alto (cercano a 1) = superficie horizontal = el jugador está encima.
            // hit.normal.y bajo (cercano a 0) = superficie vertical = colisión lateral → ignorar.
            if (hit.normal.y > minNormalY)
            {
                currentPlatform = platform;
            }
        }
        else
        {
            // Si chocamos con algo que no es plataforma (pared, otro objeto),
            // no limpiamos currentPlatform aquí — se limpia por raycast en Update.
        }
    }

    void Update()
    {
        if (currentPlatform == null) return;

        // Aplicar la velocidad de la plataforma al jugador para que se mueva junto con ella.
        // PlatformVelocity ya viene clampeada desde MovingPlatform, así que es seguro aplicarla.
        controller.Move(currentPlatform.PlatformVelocity * Time.deltaTime);

        // Verificar si el jugador sigue sobre la plataforma.
        // Si el raycast hacia abajo no golpea nada en groundCheckDistance,
        // el jugador se cayó o saltó → dejar de seguir la plataforma.
        if (!Physics.Raycast(transform.position, Vector3.down, out _, groundCheckDistance))
        {
            currentPlatform = null;
        }
    }
}