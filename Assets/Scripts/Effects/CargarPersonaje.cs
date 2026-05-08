using System.Collections.Generic;
using UnityEngine;

public class CargarPersonaje : MonoBehaviour
{
    [Header("Materiales")]
    public List<Material> listaPersonajes =
        new List<Material>();

    [Header("Renderer")]
    public Renderer personajeRenderer;

    private string key =
        "MaterialSeleccionado";

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
        }
    }

    private void Start()
    {
        if (listaPersonajes.Count == 0)
        {
            Debug.LogWarning(
                "NO HAY MATERIALES");

            return;
        }

        int indexActual =
            PlayerPrefs.GetInt(key, 0);

        indexActual =
            Mathf.Clamp(
                indexActual,
                0,
                listaPersonajes.Count - 1);

        personajeRenderer.material =
            listaPersonajes[indexActual];

        Debug.Log(
            "SKIN CARGADA: " +
            indexActual);
    }
}