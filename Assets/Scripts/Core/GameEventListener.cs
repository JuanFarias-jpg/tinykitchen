using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("El GameEvent asset al que este listener reacciona.")]
    [SerializeField] private GameEvent gameEvent;

    [Tooltip("Funciones a ejecutar cuando el evento se dispare.")]
    [SerializeField] private UnityEvent response;

    private void OnEnable()
    {
        if (gameEvent != null)
            gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (gameEvent != null)
            gameEvent.UnregisterListener(this);
    }


//llamado por GameEvent.Raise() cuando el evento se dispara.

    public void OnEventRaised()
    {
        response?.Invoke();
    }
}