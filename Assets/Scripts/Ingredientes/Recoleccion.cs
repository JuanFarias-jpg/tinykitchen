using UnityEngine;

public class Recoleccion : MonoBehaviour
{
    
    [SerializeField] GameEvent onIngredientCollected;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onIngredientCollected.Raise();
            Destroy(gameObject);
        }
    }
}
