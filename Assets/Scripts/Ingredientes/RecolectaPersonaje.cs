using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RecolectaPersonaje : MonoBehaviour
{
    [Header("Ingredientes")]
    public List<GameObject> objetosVisuales;

    [Header("Escalas finales por objeto")]
    public List<Vector3> escalasFinales;

    [Header("Rotaciones finales por objeto")]
    public List<Vector3> rotacionesFinales;

    [Header("Spawn")]
    public Transform puntoSpawn;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    public float duracion = 0.4f;
    public List<float> alturasMaximas;

    [Header("Movimiento del jugador")]
    [SerializeField] private PlayerMovement playerMovement;

    public void OnIngredientCollected(int index)
    {
        if (index < 0 || index >= objetosVisuales.Count) return;


        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        Vector3 posicion = puntoSpawn.position + Vector3.up * alturasMaximas[index];

        GameObject obj = Instantiate(objetosVisuales[index], posicion, Quaternion.identity);

        // Escala personalizada
        Vector3 escala = Vector3.one;
        if (index < escalasFinales.Count)
            escala = escalasFinales[index];

        // Rotación personalizada
        Vector3 rotacion = Vector3.zero;
        if (index < rotacionesFinales.Count)
            rotacion = rotacionesFinales[index];

        StartCoroutine(Animacion(obj, escala, rotacion));
    }

    IEnumerator Animacion(GameObject obj, Vector3 escalaObjetivo, Vector3 rotacionObjetivo)
    {
        if (animator != null)
            animator.SetTrigger("Recolecta");

        float tiempo = 0f;
        obj.transform.localScale = Vector3.zero;

        Vector3 startPos = obj.transform.position;
        Quaternion rotInicial = obj.transform.rotation;
        Quaternion rotFinal = Quaternion.Euler(rotacionObjetivo);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            // Escala
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, escalaObjetivo, t);

            // Movimiento hacia arriba
            obj.transform.position = startPos + Vector3.up * t;

            // Rotación
            obj.transform.rotation = Quaternion.Lerp(rotInicial, rotFinal, t);

            yield return null;
        }

        
        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        Destroy(obj, 1.5f);
    }
}