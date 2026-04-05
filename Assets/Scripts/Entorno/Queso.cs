using UnityEngine;

public class Queso : MonoBehaviour
{
    [Header("Curación")]
    public int healAmount = 1;
    public GameHUDManager hud;
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si es el jugador por tag
        if (other.CompareTag("Player"))
        {
                hud.Heal(healAmount);
            

            Destroy(gameObject);
        }
    }
}

