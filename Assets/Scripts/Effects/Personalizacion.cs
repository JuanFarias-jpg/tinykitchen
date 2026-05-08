using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Personalizacion : MonoBehaviour
{
    [Header("Personajes (Materiales)")]
    public List<Material> listaPersonajes = new List<Material>();

    [Header("Render del personaje")]
    public Renderer personajeRenderer;

    [Header("Audio")]
    [SerializeField] private AudioSource audio;

    [Header("Economia")]
    public IntVariable WhiteCheese;
    public int costoSkin = 5;

    [Header("UI")]
    public TextMeshProUGUI cheeseText;

    private int indexActual = 0;
    private string key = "MaterialSeleccionado";

    private bool lockedInput;

    void Start()
    {
        indexActual = PlayerPrefs.GetInt(key, 0);

        if (listaPersonajes.Count > 0)
            indexActual %= listaPersonajes.Count;
        else
            indexActual = 0;

        AplicarMaterial();
        ActualizarUI();
    }

    void Update()
    {
        if (lockedInput) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Siguiente();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Anterior();

        if (Input.GetKeyDown(KeyCode.Return))
            ComprarYContinuar();
    }

    public void Siguiente()
    {
        if (audio != null) audio.Play();

        if (listaPersonajes.Count == 0) return;

        indexActual = (indexActual + 1) % listaPersonajes.Count;
        AplicarMaterial();
    }

    public void Anterior()
    {
        if (audio != null) audio.Play();

        if (listaPersonajes.Count == 0) return;

        indexActual = (indexActual - 1 + listaPersonajes.Count) % listaPersonajes.Count;
        AplicarMaterial();
    }

    void AplicarMaterial()
    {
        if (personajeRenderer != null && listaPersonajes.Count > 0)
            personajeRenderer.material = listaPersonajes[indexActual];
    }

    public void ComprarYContinuar()
    {
        // Primera skin gratis
        if (indexActual == 0)
        {
            GuardarSkin();
            StartCoroutine(ComprarExito());
            return;
        }

        // No alcanza queso
        if (WhiteCheese.Value < costoSkin)
        {
            StartCoroutine(FlashColor(Color.red));
            return;
        }

        // Compra válida
        WhiteCheese.Value -= costoSkin;

        ActualizarUI();

        GuardarSkin();

        StartCoroutine(ComprarExito());
    }

    void GuardarSkin()
    {
        PlayerPrefs.SetInt(key, indexActual);
        PlayerPrefs.Save();
    }

    void ActualizarUI()
    {
        if (cheeseText != null)
            cheeseText.text = "White Cheese: " + WhiteCheese.Value;
    }

    IEnumerator ComprarExito()
    {
        lockedInput = true;

        yield return StartCoroutine(FlashColor(Color.green));

        GameManager.volverDePersonalizacion = true;
        SceneManager.LoadScene("Kitchen");
    }

    IEnumerator FlashColor(Color color)
    {
        lockedInput = true;

        Material mat = personajeRenderer.material;
        Color original = mat.color;

        mat.color = color;

        yield return new WaitForSeconds(1f);

        mat.color = original;

        lockedInput = false;
    }
}