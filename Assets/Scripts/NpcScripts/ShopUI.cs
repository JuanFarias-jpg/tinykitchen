using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameObject panel;

    public Button[] buttons;
    public Image[] borders;

    public IntVariable WhiteCheese;

    public int[] prices;

    public TMP_Text cheeseText;

    private int index;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        cheeseText.text = "Cheese: " + WhiteCheese.Value;

        if (Input.GetKeyDown(KeyCode.A))
            index--;

        if (Input.GetKeyDown(KeyCode.D))
            index++;

        if (index < 0) index = buttons.Length - 1;
        if (index >= buttons.Length) index = 0;

        UpdateSelection();

        if (Input.GetMouseButtonDown(1))
            Buy();

        if (Input.GetKeyDown(KeyCode.X))
            panel.SetActive(false);
    }

    public void OpenShop()
    {
        panel.SetActive(true);
        index = 0;
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < borders.Length; i++)
            borders[i].enabled = i == index;
    }

    void Buy()
    {
        int price = prices[index];

        if (WhiteCheese.Value >= price)
        {
            WhiteCheese.Value -= price;
            Debug.Log("Compraste skin " + index);
        }
        else
        {
            Debug.Log("No tienes suficiente queso");
        }
    }
}