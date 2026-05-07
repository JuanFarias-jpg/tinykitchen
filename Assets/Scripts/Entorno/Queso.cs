using UnityEngine;

/// <summary>
/// Queso — Objeto de curación que el jugador puede recolectar.
/// VERSIÓN CORREGIDA (Fase 1, Entrega Final).
///
/// CAMBIO RESPECTO A LA VERSIÓN ANTERIOR:
///   ✗ Eliminada la referencia directa a GameHUDManager (problema #3 de la bitácora).
///     Antes: hud.Heal(healAmount) → acoplamiento con el HUD, rompe arquitectura SO.
///   ✓ Ahora: llama PlayerHealth.Heal(healAmount) en el componente del jugador.
///     El HUD refleja el cambio automáticamente porque lee playerHP.Value en Update().
///
/// QUIÉN LO LLAMA: nadie. OnTriggerEnter reacciona al contacto físico con el Player.
/// CUÁNDO SE CONECTA: ya funciona. Solo reemplazar este archivo en el proyecto.
/// </summary>
public class Queso : MonoBehaviour
{
    [Header("Curación")]
    public int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Buscar PlayerHealth en el objeto que tocó el trigger (o en sus padres).
        // GetComponentInParent es más robusto si el Collider del jugador está en un hijo.
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}