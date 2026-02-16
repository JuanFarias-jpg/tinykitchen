using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    [Header("Assign")]
    public Transform pitchPivot; 

    [Header("Settings")]
    public float sensitivity = 180f;
    public float minPitch = -40f;
    public float maxPitch = 70f;
    public bool invertY = false;

    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = pitchPivot ? pitchPivot.localEulerAngles.x : 0f;

        
        if (pitch > 180f) pitch -= 360f;
    }

    void Update()
    {
        if (!pitchPivot) return;

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * sensitivity * Time.deltaTime;

        float ySign = invertY ? 1f : -1f;
        pitch += my * sensitivity * Time.deltaTime * ySign;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);        // izquierda/derecha
        pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f); // arriba/abajo
    }
}
