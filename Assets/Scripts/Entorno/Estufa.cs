using UnityEngine;
using System.Collections;

public class Estufa : MonoBehaviour
{
    [Header("Tiempo")]
    public float onTime = 5f;
    public float offTime = 3f;

    [Header("Estado")]
    public bool isOn = false;

    [Header("Referencias")]
    [SerializeField] private GameObject fireVFX;

    [Header("Evento de Daño")]
    public GameEvent OnPlayerDamage;

    [Header("Daño")]
    public float damageCooldown = 0.6f;

    private float damageTimer = 0f;
    private bool playerInside = false;

    private void Start()
    {
        StartCoroutine(StoveCycle());
        UpdateStove();
    }

    private void Update()
    {
        if (!isOn || !playerInside) return;

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageCooldown)
        {
            DealDamage();
        }
    }

    IEnumerator StoveCycle()
    {
        while (true)
        {
            isOn = true;
            UpdateStove();
            yield return new WaitForSeconds(onTime);

            isOn = false;
            UpdateStove();
            yield return new WaitForSeconds(offTime);
        }
    }

    void UpdateStove()
    {
        if (fireVFX != null)
            fireVFX.SetActive(isOn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOn) return;

        if (other.CompareTag("Player"))
        {
            playerInside = true;

            
            DealDamage();

            damageTimer = 0f;
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

    void DealDamage()
    {
        if (OnPlayerDamage != null)
            OnPlayerDamage.Raise();

        damageTimer = 0f;
    }
}