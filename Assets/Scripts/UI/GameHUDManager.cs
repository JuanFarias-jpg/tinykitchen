using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    [Header("VARIABLES (ScriptableObjects) — solo lectura")]
    public IntVariable playerHP;
    public FloatVariable gameTimer;
    public FloatVariable freezeLevel;

    [Header("TIMER")]
    public TextMeshProUGUI timerText;

    [Header("CORAZONES (VIDA)")]
    public Image[] hearts;
    public Color normalColor = Color.white;
    public Color emptyColor  = Color.black;
    public Color freezeColor = Color.cyan;

    [Header("FREEZE")]
    public float maxFreeze = 100f;

    [Header("OBJETIVOS (MAPA / UI)")]
    public GameObject[] objectiveImages;

    [Header("ZONAS")]
    public GameObject zona1;
    public GameObject zona2;
    public GameObject zona3;
    public GameObject zona4;
    public GameObject zona5;

    private int ingredientes = 0;

    private void Start()
    {
        // Activar solo la zona 1 al inicio.
        if (zona1 != null) zona1.SetActive(true);
        if (zona2 != null) zona2.SetActive(false);
        if (zona3 != null) zona3.SetActive(false);
        if (zona4 != null) zona4.SetActive(false);
        if (zona5 != null) zona5.SetActive(false);

        foreach (GameObject obj in objectiveImages)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void Update()
    {

        UpdateTimer();
        UpdateHearts();
        UpdateFreezeEffect();
    }

    private void UpdateTimer()
    {
        if (timerText == null || gameTimer == null) return;

        float time = Mathf.Max(0f, gameTimer.Value);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void UpdateHearts()
    {
        if (hearts == null || playerHP == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].color = (i < playerHP.Value) ? normalColor : emptyColor;
        }
    }

    private void UpdateFreezeEffect()
    {
        if (freezeLevel == null) return;

        float freezePercent = freezeLevel.Value / maxFreeze;

        if (freezePercent > 0.5f)
        {
            foreach (Image heart in hearts)
            {
                if (heart != null && heart.color != emptyColor)
                    heart.color = Color.Lerp(normalColor, freezeColor, freezePercent);
            }
        }
    }

    public void OnIngredientCollected()
    {
        if (objectiveImages != null && ingredientes < objectiveImages.Length)
        {
            if (objectiveImages[ingredientes] != null)
                objectiveImages[ingredientes].SetActive(true);
        }

        ingredientes++;

        switch (ingredientes)
        {
            case 1:
                if (zona2 != null) zona2.SetActive(true);
                break;
            case 2:
                if (zona3 != null) zona3.SetActive(true);
                break;
            case 3:
                if (zona4 != null) zona4.SetActive(true);
                break;
            case 4:
                if (zona5 != null) zona5.SetActive(true);
                break;
        }
    }
}