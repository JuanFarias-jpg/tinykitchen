using UnityEngine;


public class PlayerPlatform : MonoBehaviour
{
    private CharacterController controller;

    private MovingPlatform currentPlatform;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();

        if (platform != null)
        {
           
            if (hit.normal.y > 0.5f)
            {
                currentPlatform = platform;
            }
        }
    }

    void Update()
    {
        if (currentPlatform != null)
        {
            
            controller.Move(currentPlatform.PlatformVelocity * Time.deltaTime);

            
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                currentPlatform = null;
            }
        }
    }
}