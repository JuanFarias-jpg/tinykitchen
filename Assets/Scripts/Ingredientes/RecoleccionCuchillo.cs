using UnityEngine;

public class RecoleccionCuchillo : MonoBehaviour
{

    [SerializeField] GameObject Cuchillo;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cuchillo.SetActive(true);
            Destroy(gameObject);
        }
    }
}

