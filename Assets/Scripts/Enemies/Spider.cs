using UnityEngine;

public class Spider : MonoBehaviour
{
    public float damageInterval = 1f;
    private float damageTimer = 0f;

    public GameEvent OnPlayerDamaged;
    public AudioSource audioAttack;

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            DoDamage();
            damageTimer = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInside)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                DoDamage();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            damageTimer = 0f;
        }
    }

    void DoDamage()
    {
        if (OnPlayerDamaged != null)
            OnPlayerDamaged.Raise();

        if (audioAttack != null)
            audioAttack.Play();
    }
}