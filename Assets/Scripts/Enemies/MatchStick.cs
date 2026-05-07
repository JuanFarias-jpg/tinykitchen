using UnityEngine;
using System.Collections;

public class MatchStick : MonoBehaviour
{
    [Header("Configuración de fuego")]
    [Tooltip("Duración en segundos del efecto de fuego en el jugador.")]
    public float fireDuration = 10f;

    [Header("Referencias en el jugador")]
    [Tooltip("Collider en el jugador con tag 'Fire'. Debe estar desactivado por defecto.")]
    public Collider fireCollider;

    [Tooltip("Efecto de partículas de fuego en el jugador (opcional).")]
    public ParticleSystem fireVFX;

    [Header("Audio")]
    public AudioSource pickupSound;

    [Header("Visual del cerillo")]
    [Tooltip("Desactiva el renderer del cerillo al ser recogido (antes de destruirse).")]
    public Renderer matchRenderer;

    private bool recogido = false;

    private void OnTriggerEnter(Collider other)
    {
        if (recogido) return;
        if (!other.CompareTag("Player")) return;

        recogido = true;

        if (pickupSound != null) pickupSound.Play();


        StartCoroutine(ActivarFuego(other.gameObject));
    }

    private IEnumerator ActivarFuego(GameObject player)
    {
        // Ocultar el cerillo inmediatamente.
        if (matchRenderer != null) matchRenderer.enabled = false;

        // Activar collider de fuego en el jugador.
        if (fireCollider != null) fireCollider.enabled = true;
        if (fireVFX     != null) fireVFX.Play();

        // Esperar la duración del efecto.
        yield return new WaitForSeconds(fireDuration);

        // Desactivar el efecto de fuego.
        if (fireCollider != null) fireCollider.enabled = false;
        if (fireVFX     != null) fireVFX.Stop();

        // Destruir el cerillo.
        Destroy(gameObject);
    }
}