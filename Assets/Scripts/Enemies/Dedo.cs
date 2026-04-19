using UnityEngine;

public class BossDedo: MonoBehaviour
{
    [Header("Referencia a la Mano")]
    public BossHand bossHand;

    [Header("Tag que puede quemar")]
    public string burnTag = "Fire";

    [Header("Visual Opcional")]
    public GameObject burnedVisual;

    private bool burned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (burned) return;

        if (other.CompareTag(burnTag))
        {
            BurnFinger();
        }
    }

    void BurnFinger()
    {
        burned = true;

        if (burnedVisual != null)
            burnedVisual.SetActive(true);

        if (bossHand != null)
            bossHand.BurnFinger();

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}