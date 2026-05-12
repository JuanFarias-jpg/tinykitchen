using UnityEngine;

public class CameraTrailer : MonoBehaviour
{
    [Header("Puntos de la cámara (waypoints)")]
    public Transform[] points;

    [Header("Velocidad de movimiento")]
    public float speed = 5f;

    [Header("Velocidad de rotación")]
    public float rotationSpeed = 180f;

    private int currentIndex = 0;

    void Start()
    {
        if (points.Length > 0)
        {
            transform.position = points[0].position;
            transform.rotation = points[0].rotation;
        }
    }

    void Update()
    {
        if (points.Length == 0) return;

        Transform targetPoint = points[currentIndex];

        // Movimiento seguro (SIEMPRE llega al punto)
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        // Rotación segura (en grados por segundo)
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetPoint.rotation,
            rotationSpeed * Time.deltaTime
        );

        // Cambiar cuando llegue EXACTAMENTE al punto
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            currentIndex++;

            if (currentIndex >= points.Length)
                currentIndex = 0;
        }
    }
}