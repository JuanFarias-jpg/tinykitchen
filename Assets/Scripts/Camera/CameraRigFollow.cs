using UnityEngine;

public class CameraRigFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    void LateUpdate()
    {
        if (!player) return;
        transform.position = player.position + offset;
        // NO tocamos rotación aquí
    }
}
