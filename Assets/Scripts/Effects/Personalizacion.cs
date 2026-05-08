using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Personalizacion : MonoBehaviour
{
    [Header("Materiales")]
    public List<Material> listaPersonajes =
        new List<Material>();

    [Header("Renderer")]
    public Renderer personajeRenderer;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Economía")]
    public IntVariable WhiteCheese;
    public int costoSkin = 5;

    [Header("UI")]
    public TextMeshProUGUI cheeseText;

    private int indexActual = 0;

    private string key =
        "MaterialSeleccionado";

    private bool lockedInput = false;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (personajeRenderer == null)
        {
            personajeRenderer =
                GetComponentInChildren<Renderer>();
        }

        if (personajeRenderer == null)
        {
            Debug.LogError(
                "NO HAY RENDERER");

            enabled = false;
            return;
        }
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        indexActual =
            PlayerPrefs.GetInt(key, 0);

        if (listaPersonajes.Count > 0)
        {
            indexActual =
                Mathf.Clamp(
                    indexActual,
                    0,
                    listaPersonajes.Count - 1);
        }

        AplicarMaterial();

        ActualizarUI();

        Debug.Log(
            "SKIN ACTUAL: " +
            indexActual);
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (lockedInput)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Siguiente();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Anterior();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ComprarYContinuar();
        }
    }

    // =========================
    // SIGUIENTE
    // =========================

    public void Siguiente()
    {
        if (listaPersonajes.Count == 0)
            return;

        if (audioSource != null)
            audioSource.Play();

        indexActual++;

        if (indexActual >= listaPersonajes.Count)
        {
            indexActual = 0;
        }

        AplicarMaterial();

        Debug.Log(
            "SKIN SELECCIONADA: " +
            indexActual);
    }

    // =========================
    // ANTERIOR
    // =========================

    public void Anterior()
    {
        if (listaPersonajes.Count == 0)
            return;

        if (audioSource != null)
            audioSource.Play();

        indexActual--;

        if (indexActual < 0)
        {
            indexActual =
                listaPersonajes.Count - 1;
        }

        AplicarMaterial();

        Debug.Log(
            "SKIN SELECCIONADA: " +
            indexActual);
    }

    // =========================
    // APLICAR MATERIAL
    // =========================

    private void AplicarMaterial()
    {
        if (personajeRenderer == null)
            return;

        if (listaPersonajes.Count == 0)
            return;

        personajeRenderer.material =
            listaPersonajes[indexActual];
    }

    // =========================
    // COMPRAR
    // =========================

    public void ComprarYContinuar()
    {
        if (lockedInput)
            return;

        lockedInput = true;

        // =========================
        // SKIN GRATIS
        // =========================

        if (indexActual == 0)
        {
            GuardarSkin();

            StartCoroutine(
                CompraExitosa());

            return;
        }

        // =========================
        // NO ALCANZA
        // =========================

        if (WhiteCheese.Value < costoSkin)
        {
            StartCoroutine(
                FlashColor(Color.red));

            return;
        }

        // =========================
        // COMPRA
        // =========================

        WhiteCheese.Value -= costoSkin;

        ActualizarUI();

        GuardarSkin();

        StartCoroutine(
            CompraExitosa());
    }

    // =========================
    // GUARDAR SKIN
    // =========================

    private void GuardarSkin()
    {
        PlayerPrefs.SetInt(
            key,
            indexActual);

        PlayerPrefs.Save();

        Debug.Log(
            "GUARDANDO SKIN: " +
            indexActual);
    }

    // =========================
    // UI
    // =========================

    private void ActualizarUI()
    {
        if (cheeseText != null)
        {
            cheeseText.text =
                "Yellow Cheese: " +
                WhiteCheese.Value;
        }
    }

    // =========================
    // COMPRA EXITOSA
    // =========================

    IEnumerator CompraExitosa()
    {
        yield return StartCoroutine(
            FlashColor(Color.green));

        yield return new WaitForSeconds(
            0.2f);

        // IMPORTANTE
        PlayerPrefs.SetInt(
            "ComingFromPersonalizacion",
            1);

        PlayerPrefs.Save();

        SceneManager.LoadScene(
            "Kitchen 1");
    }

    // =========================
    // FLASH
    // =========================

    IEnumerator FlashColor(Color color)
    {
        if (personajeRenderer == null)
        {
            lockedInput = false;
            yield break;
        }

        Material mat =
            personajeRenderer.material;

        Color original =
            mat.color;

        mat.color = color;

        yield return new WaitForSeconds(
            0.5f);

        mat.color = original;

        lockedInput = false;
    }
}