using UnityEngine;



public class UIToggle : MonoBehaviour
{
    [Header("Canvas a ocultar")]
    [Tooltip("Arrastra aquí el GameObject raíz del HUD (el que tiene el componente Canvas principal). Al desactivarlo se oculta toda la UI de golpe.")]
    [SerializeField] private GameObject hudCanvas;

    private bool hudVisible = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ToggleHUD();
    }

    void ToggleHUD()
    {
        hudVisible = !hudVisible;
        hudCanvas.SetActive(hudVisible);
        Debug.Log($"[UIToggle] HUD: {(hudVisible ? "visible" : "oculto")}");
    }

    // Muestra en pantalla el estado actual igual que CameraDirector
    void OnGUI()
    {
        if (hudVisible) return; // Solo muestra el aviso cuando la UI está oculta

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 16;
        style.normal.textColor = new Color(1f, 0.4f, 0.4f);

        GUI.Box(new Rect(10, 50, 160, 30), "HUD: OCULTO [0]", style);
    }
}