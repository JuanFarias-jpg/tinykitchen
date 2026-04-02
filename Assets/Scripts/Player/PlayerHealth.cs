using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Variables")]
    public IntVariable playerHP;

    [Header("Eventos")]
    public GameEvent OnPlayerDied;

    [Header("Configuración")]
    public int damageAmount = 1;
    public float invincibilityTime = 1f;

    private bool isInvincible = false;

    public void TakeDamage()
    {
        if (isInvincible) return;

        playerHP.Value -= damageAmount;

        Debug.Log("Vida jugador: " + playerHP.Value);

        if (playerHP.Value <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFrames());
    }

    void Die()
    {
        Debug.Log("Jugador murió");

        OnPlayerDied.Raise();
    }

    System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
}