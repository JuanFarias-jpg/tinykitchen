using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("Texto que aparecerá después")]
    public TextMeshProUGUI delayedText;

    [Header("Tiempo")]
    public float delay = 4f;

    private void Start()
    {
        if (delayedText != null)
            delayedText.gameObject.SetActive(false);

        StartCoroutine(ShowTextLater());
    }

    IEnumerator ShowTextLater()
    {
        yield return new WaitForSecondsRealtime(delay);

        if (delayedText != null)
            delayedText.gameObject.SetActive(true);
    }

    
    public void VolverInicio()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("INICIO");
    }
}