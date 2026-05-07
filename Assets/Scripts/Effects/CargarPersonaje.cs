using System.Collections.Generic;
using UnityEngine;

public class CargarPersonaje : MonoBehaviour
{
    public List<Material> listaPersonajes = new List<Material>();
    public Renderer personajeRenderer;

    private string key = "MaterialSeleccionado";

    void Start()
    {
        if (listaPersonajes.Count == 0 || personajeRenderer == null)
        {
            Debug.LogWarning("Faltan materiales o Renderer");
            return;
        }

        int indexActual = PlayerPrefs.GetInt(key, 0);

        
        indexActual = Mathf.Clamp(indexActual, 0, listaPersonajes.Count - 1);

        personajeRenderer.material = listaPersonajes[indexActual];
    }
}