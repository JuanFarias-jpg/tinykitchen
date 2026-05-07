using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Variable SO")]
    [Tooltip("FloatVariable compartido con GameHUDManager. initialValue = 1200.")]
    public FloatVariable gameTimer;

    [Header("Evento SO")]
    [Tooltip("Se dispara cuando el tiempo llega a 0. GameManager escucha este evento.")]
    public GameEvent OnTimeExpired;

    private bool expired = false;

    private void Update()
    {
   
        if (expired) return;

        gameTimer.Value -= Time.deltaTime;

        if (gameTimer.Value <= 0f)
        {
            gameTimer.Value = 0f;
            expired = true;
            OnTimeExpired?.Raise();
        }
    }

    public void PauseTimer()  => expired = true;
    public void ResumeTimer() => expired = false;
}