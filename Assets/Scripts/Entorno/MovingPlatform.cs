using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Puntos")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movimiento")]
    public float speed = 2f;

    [Header("Desfase")]
    public float offset = 0f;

    // Velocidad máxima (unidades/s) que la plataforma puede reportarle al jugador.
    [Header("Seguridad")]
    [SerializeField] private float maxReportedSpeed = 30f;

    private float time;
    private Vector3 lastPosition;
    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        // Inicializar lastPosition desde la posición real del primer frame,
        // respetando el offset. Sin esto, el primer frame calcula:
        //   (posición_con_offset - posición_de_spawn) / deltaTime
        // lo que produce una velocidad enorme si offset != 0.
        if (pointA != null && pointB != null)
        {
            float t = Mathf.PingPong(offset, 1f);
            lastPosition = Vector3.Lerp(pointA.position, pointB.position, t);
        }
        else
        {
            lastPosition = transform.position;
        }
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        time += Time.deltaTime * speed;

        float t = Mathf.PingPong(time + offset, 1f);

        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);

        Vector3 rawVelocity = (transform.position - lastPosition) / Time.deltaTime;

        // ClampMagnitude conserva la dirección exacta pero limita la magnitud.
        // Así un frame de lag o el primer frame con offset no produce velocidad explosiva.
        PlatformVelocity = Vector3.ClampMagnitude(rawVelocity, maxReportedSpeed);

        lastPosition = transform.position;
    }
}