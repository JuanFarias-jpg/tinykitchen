using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "TinyKitchen/Game Event")]
public class GameEvent : ScriptableObject
{

//lista de listeners actualmente suscritos
    private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

//dispara el evento y recorre la lista en reversa por si algun listener se desuscribe durante la ejecucion.

    public void Raise()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised();
        }
    }

//llamado por GameListener en OnEnable
    public void RegisterListener(GameEventListener listener)
    {
        if (!_listeners.Contains(listener))
            _listeners.Add(listener);
    }


//llamado por GameListener en OnDisable
    public void UnregisterListener(GameEventListener listener)
    {
        _listeners.Remove(listener);
    }
}