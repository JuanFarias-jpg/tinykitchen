using UnityEngine;
using TMPro;

public class QuesoMoneda : MonoBehaviour
{
    [Header("Moneda")]
    public IntVariable WhiteCheese;
    public int amount = 1;
    public AudioSource moneda;

    [Header("UI")]
    public TextMeshProUGUI cheeseText;

    private void Start()
    {
        ActualizarUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (WhiteCheese != null)
        {
            WhiteCheese.Value += amount;
            moneda.Play();
            ActualizarUI();
        }

        Destroy(gameObject);
    }

    void ActualizarUI()
    {
        if (cheeseText != null && WhiteCheese != null)
        {
            cheeseText.text =  WhiteCheese.Value.ToString();
        }
    }
}