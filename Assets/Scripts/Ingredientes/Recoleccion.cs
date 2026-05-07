using UnityEngine;

public class Recoleccion : MonoBehaviour
{
    [SerializeField] GameEvent onIngredientCollected;
    public int NumeroObjeto;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RecolectaPersonaje efecto = FindObjectOfType<RecolectaPersonaje>();

            if (efecto != null)
            {
                efecto.OnIngredientCollected(NumeroObjeto);
            }

            onIngredientCollected.Raise();

            Destroy(gameObject);
        }
    }
}