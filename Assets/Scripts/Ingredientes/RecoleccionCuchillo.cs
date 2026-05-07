using UnityEngine;

public class RecoleccionCuchillo : MonoBehaviour
{
    [SerializeField] GameObject Rata;
    [SerializeField] GameObject Cuchillo;
    [SerializeField] PlayerCombat playerc;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rata.SetActive(true);
            playerc.damage = 3;
            Cuchillo.SetActive(true);
            Destroy(gameObject);
        }
    }
}

