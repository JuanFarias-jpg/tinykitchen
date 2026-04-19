using UnityEngine;

public class FreezeZone : MonoBehaviour
{
    [Header("Freeze")]
    public FloatVariable freezeLevel;
    public float freezeSpeed = 20f;
    public float unfreezeSpeed = 30f;
    public float maxFreeze = 100f;

    [Header("Player Speed")]
    public FloatVariable playerSpeed;
    public float slowAmount = 0.5f;
    public float minSpeed = 1f;
    public float maxSpeed = 7f;

    [Header("Tiempo para daño")]
    public float timeToDamage = 30f;
    private float freezeTimer = 0f;

    [Header("Tiempo para slow")]
    public float timeToSlow = 10f;
    private float slowTimer = 0f;

    [Header("Eventos")]
    public GameEvent OnFreezeWarning;
    public GameEvent OnPlayerDamage;

    private bool playerInside = false;

    private void Update()
    {
        if (playerInside)
        {
            
            freezeLevel.Value += freezeSpeed * Time.deltaTime;
            freezeLevel.Value = Mathf.Clamp(freezeLevel.Value, 0f, maxFreeze);

            
            if (freezeLevel.Value >= maxFreeze * 0.5f)
            {
                OnFreezeWarning?.Raise();
            }

            
            freezeTimer += Time.deltaTime;
            if (freezeTimer >= timeToDamage)
            {
                OnPlayerDamage?.Raise();
                freezeTimer = 0f;
            }

            
            slowTimer += Time.deltaTime;
            if (slowTimer >= timeToSlow)
            {
                playerSpeed.Value -= slowAmount;

                
                playerSpeed.Value = Mathf.Clamp(playerSpeed.Value, minSpeed, maxSpeed);

                Debug.Log("PLAYER MAS LENTO: " + playerSpeed.Value);

                slowTimer = 0f;
            }
        }
        else
        {
            
            if (freezeLevel.Value > 0)
            {
                freezeLevel.Value -= unfreezeSpeed * Time.deltaTime;
                freezeLevel.Value = Mathf.Clamp(freezeLevel.Value, 0f, maxFreeze);
            }

            
            playerSpeed.Value += (slowAmount * 2f) * Time.deltaTime;

            
            playerSpeed.Value = Mathf.Clamp(playerSpeed.Value, minSpeed, maxSpeed);

            freezeTimer = 0f;
            slowTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}