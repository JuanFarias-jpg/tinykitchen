using UnityEngine;

public class BossCollider : MonoBehaviour
{
    public GameEvent OnPlayerDamaged;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerDamaged.Raise();
        }
    }
}