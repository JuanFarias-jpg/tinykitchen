using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    [Header(" VARIABLES (ScriptableObjects) ")]
    public IntVariable playerHP;
    public FloatVariable gameTimer;
    public FloatVariable freezeLevel;

    [Header(" TIMER ")]
    public TextMeshProUGUI timerText;

    [Header(" CORAZONES (VIDA) ")]
    public Image[] hearts;
    public Color normalColor = Color.white;
    public Color emptyColor = Color.black;
    public Color freezeColor = Color.cyan;

    [Header(" FREEZE ")]
    public float maxFreeze = 100f;

    [Header(" OBJETIVOS (MAPA / UI) ")]
    public GameObject[] objectiveImages; 

    [Header(" ZONAS ")]
    public GameObject zona1;
    public GameObject zona2;
    public GameObject zona3;
    public GameObject zona4;
    public GameObject zona5;

    private int ingredientes = 0;

    void Start()
    {
        gameTimer.Value = 0;
        playerHP.Value = 5;
        zona1.SetActive(true);
        zona2.SetActive(false);
        zona3.SetActive(false);
        zona4.SetActive(false);
        zona5.SetActive(false);
        foreach (GameObject obj in objectiveImages)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        gameTimer.Value += Time.deltaTime;

        UpdateTimer();
        UpdateHearts();
        UpdateFreezeEffect();
    }

    void UpdateTimer()
    {
        float time = gameTimer.Value;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }


    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHP.Value)
            {
                hearts[i].color = normalColor;
            }
            else
            {
                hearts[i].color = emptyColor;
            }
        }
    }

 
    void UpdateFreezeEffect()
    {
        float freezePercent = freezeLevel.Value / maxFreeze;

        if (freezePercent > 0.5f)
        {
            // cambia color de corazones a azul
            foreach (Image heart in hearts)
            {
                if (heart.color != emptyColor)
                {
                    heart.color = Color.Lerp(normalColor, freezeColor, freezePercent);
                }
            }
        }
    }

    
    public void OnIngredientCollected()
    {
        if (ingredientes < objectiveImages.Length)
        {
            objectiveImages[ingredientes].SetActive(true);
        }

        ingredientes++;

        switch (ingredientes)
        {
            case 1:
                zona2.SetActive(true);
                break;

            case 2:
                zona3.SetActive(true);
                break;

            case 4:
                zona4.SetActive(true);
                break;
            case 5:
                zona5.SetActive(true);
                break;
        }
    }

    
    public bool IsDead()
    {
        return playerHP.Value <= 0;
    }

 
    public void TakeDamage(int damage)
    {
        playerHP.Value -= damage;
        playerHP.Value = Mathf.Clamp(playerHP.Value, 0, hearts.Length);
    }

   
    public void Heal(int amount)
    {
        playerHP.Value += amount;
        playerHP.Value = Mathf.Clamp(playerHP.Value, 0, hearts.Length);
    }
}