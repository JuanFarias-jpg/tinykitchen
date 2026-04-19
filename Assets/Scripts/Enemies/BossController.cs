using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss HP Global")]
    public IntVariable bossHP;

    [Header("Fases")]
    public int phase2Threshold = 50;

    [Header("Eventos")]
    public GameEvent OnBossPhaseChange;
    public GameEvent OnBossDefeated;

    private bool phase2Triggered = false;
    private bool defeated = false;

    private void Start()
    {
        defeated = false;
        phase2Triggered = false;
    }

    public void DamageBoss(int amount)
    {
        if (defeated) return;

        bossHP.Value -= amount;
        bossHP.Value = Mathf.Max(0, bossHP.Value);

        Debug.Log("Boss HP: " + bossHP.Value);

        if (!phase2Triggered && bossHP.Value <= phase2Threshold)
        {
            phase2Triggered = true;
            OnBossPhaseChange?.Raise();
        }

        if (bossHP.Value <= 0)
        {
            DefeatBoss();
        }
    }

    void DefeatBoss()
    {
        if (defeated) return;

        defeated = true;


        OnBossDefeated?.Raise();
    }
}