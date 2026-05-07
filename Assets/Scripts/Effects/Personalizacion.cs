using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Personalizacion : MonoBehaviour
{
    [Header("Personajes (Materiales)")]
    public List<Material> listaPersonajes = new List<Material>();

    [Header("Render del personaje")]
    public Renderer personajeRenderer;

    [Header("Audio")]
    [SerializeField]private AudioSource audio;

    private int indexActual = 0;
    private string key = "MaterialSeleccionado";

    void Start()
    {
        indexActual = PlayerPrefs.GetInt(key, 0);

       
        if (listaPersonajes.Count > 0)
        {
            indexActual = indexActual % listaPersonajes.Count;
        }
        else
        {
            indexActual = 0;
        }

        AplicarMaterial();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Siguiente();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Anterior();
        }
        if(Input.GetKeyDown(KeyCode.Return)) 
        { 
            Continue();
        }
    }

    public void Siguiente()
    {
        if (audio != null)
            audio.Play();
        if (listaPersonajes.Count == 0) return;

        indexActual = (indexActual + 1) % listaPersonajes.Count;
        GuardarYAplicar();
    }

    public void Anterior()

    {
        if (audio != null)
            audio.Play();
        if (listaPersonajes.Count == 0) return;

        indexActual = (indexActual - 1 + listaPersonajes.Count) % listaPersonajes.Count;
        GuardarYAplicar();
    }

    void AplicarMaterial()
    {
        if (personajeRenderer != null && listaPersonajes.Count > 0)
        {
            indexActual = Mathf.Clamp(indexActual, 0, listaPersonajes.Count - 1);
            personajeRenderer.material = listaPersonajes[indexActual];
        }
    }

    void GuardarYAplicar()
    {
        PlayerPrefs.SetInt(key, indexActual);
        PlayerPrefs.Save();
        AplicarMaterial();
    }

    public void Continue()
    {
        Debug.Log("BOTON FUNCIONA");
        if (audio != null)
            audio.Play();
        //StartCoroutine(Segundo());
        SceneManager.LoadScene("Kitchen");
    }

}