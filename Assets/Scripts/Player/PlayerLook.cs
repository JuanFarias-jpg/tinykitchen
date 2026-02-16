using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    public Transform cameraPivot;   
    public Transform cam;           

    [Header("Camera Settings")]
    public float distance = 4.5f;
    public float height = 1.6f;
    public float mouseSensitivity = 180f;
    public float smooth = 14f;
    public float minPitch = -35f;
    public float maxPitch = 65f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * mouseSensitivity * Time.deltaTime;
        pitch -= my * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Pivot rotation
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Camera desired position -  put behind pivot
        Vector3 targetPos = cameraPivot.position
                            + Vector3.up * (height - 1.0f)  
                            - cameraPivot.forward * distance;

        
        cam.position = Vector3.Lerp(cam.position, targetPos, smooth * Time.deltaTime);
        cam.rotation = Quaternion.Lerp(cam.rotation, cameraPivot.rotation, smooth * Time.deltaTime);
    }
}
