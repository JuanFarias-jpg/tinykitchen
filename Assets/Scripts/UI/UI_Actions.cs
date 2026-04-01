using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    [Header("=== VARIABLES (ScriptableObjects) ===")]
    public IntVariable playerHP;
    public FloatVariable gameTimer;
    public FloatVariable freezeLevel;

    [Header("=== UI ===")]
    public Image healthFill;
    public Image freezeFill;
    public TextMeshProUGUI timerText;

    [Header("Ingredientes UI")]
    public Image[] ingredientSlots;

    [Header("Win/Lose UI")]
    

    [Header("=== ZONAS ===")]
    public GameObject zona1;
    public GameObject zona2;
    public GameObject zona3;
    public GameObject zona4;

    private int ingredientes = 0;

    [Header("CONFIG")]
    public int maxHP = 100;
    public float maxFreeze = 100f;

    void Start()
    {
        
        zona1.SetActive(true);
        zona2.SetActive(false);
        zona3.SetActive(false);
        zona4.SetActive(false);

        
    }

    void Update()
    {
        gameTimer.Value += Time.deltaTime;
        UpdateHealth();
        UpdateTimer();
        UpdateFreeze();
    }

    void UpdateHealth()
    {
        float value = (float)playerHP.Value / maxHP;
        healthFill.fillAmount = value;
    }

    void UpdateTimer()
    {
        float time = gameTimer.Value;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void UpdateFreeze()
    {
        float value = freezeLevel.Value / maxFreeze;
        freezeFill.fillAmount = value;
    }

    
    public void OnIngredientCollected()
    {
        if (ingredientes < ingredientSlots.Length)
        {
            ingredientSlots[ingredientes].color = Color.white;
            ingredientes++;
        }

        
        switch (ingredientes)
        {
            case 1:
                zona2.SetActive(true);
                Debug.Log("Zona 2 desbloqueada");
                break;

            case 2:
                zona3.SetActive(true);
                Debug.Log("Zona 3 desbloqueada");
                break;

            case 3:
                zona4.SetActive(true);
                Debug.Log("Zona 4 desbloqueada");
                break;
        }
    }

    
    
}