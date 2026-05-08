using UnityEngine;

public class NPCHeadLookAt : MonoBehaviour
{
    [SerializeField] private Transform headBone;
    [SerializeField] private Transform target;

    [Header("SpeedHead")]
    [SerializeField] private float rotationSpeed = 5f;

    private bool isLooking;

    private void Update()
    {
        if (!isLooking || target == null || headBone == null)
            return;

        //dirección hacia el objetivo
        Vector3 direction = target.position - headBone.position;

        //rotación hacia el objetivo
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        //suavizar la rotación
        headBone.rotation = Quaternion.Slerp(
            headBone.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    public void LookAtTarget(Transform targetTransform)
    {
        target = targetTransform;
        isLooking = true;
    }

    public void StopLooking()
    {
        isLooking = false;
    }
}