using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image npcImage;
    [SerializeField] private Image mechanicsImage;

    [Header("Typing")]
    [SerializeField] private float normalSpeed = 0.04f;
    [SerializeField] private float fastSpeed = 0.005f;

    [Header("Text Limit")]
    [SerializeField] private int maxCharactersPerPage = 180;

    private bool isTyping;
    private bool speedUp;
    private Coroutine typingCoroutine;

    private System.Action finishCallback;

    private string fullText;
    private int currentIndex;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        // Mantener E = acelerar texto
        speedUp = Input.GetKey(KeyCode.E);

        // Si ya terminó texto y presiona E = cerrar
        if (!isTyping && Input.GetKeyDown(KeyCode.E))
        {
            if (currentIndex < fullText.Length)
            {
                typingCoroutine = StartCoroutine(TypePage());
            }
            else
            {
                CloseDialogue();
            }
        }
    }

    public void ShowDialogue(
        string text,
        Sprite npcSprite,
        Sprite mechanicSprite,
        System.Action onFinish = null)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        panel.SetActive(true);

        npcImage.sprite = npcSprite;
        mechanicsImage.sprite = mechanicSprite;

        finishCallback = onFinish;

        fullText = text;
        currentIndex = 0;

        typingCoroutine = StartCoroutine(TypePage());
    }

    private IEnumerator TypePage()
    {
        isTyping = true;
        dialogueText.text = "";

        int count = 0;

        while (currentIndex < fullText.Length &&
               count < maxCharactersPerPage)
        {
            dialogueText.text += fullText[currentIndex];
            currentIndex++;
            count++;

            yield return new WaitForSeconds(
                speedUp ? fastSpeed : normalSpeed);
        }

        isTyping = false;
    }

    private void CloseDialogue()
    {
        panel.SetActive(false);

        finishCallback?.Invoke();
        finishCallback = null;
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public bool IsOpen()
    {
        return panel.activeSelf;
    }
}