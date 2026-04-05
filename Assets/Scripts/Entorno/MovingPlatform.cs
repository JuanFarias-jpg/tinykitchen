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

    private float time;

    private Vector3 lastPosition;
    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        time += Time.deltaTime * speed;

        float t = Mathf.PingPong(time + offset, 1f);

        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);

        
        PlatformVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}