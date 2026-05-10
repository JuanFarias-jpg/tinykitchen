using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3.x (Unity 6) — antes era solo "using Cinemachine"

// CameraDirector.cs
// Uso exclusivo para grabación del tráiler — no forma parte del juego final.
// Adjunta este script a un GameObject vacío llamado "CameraDirector" en la escena.
// Desactiva el GameObject antes de hacer build o al terminar de grabar.

public class CameraDirector : MonoBehaviour
{
    [Header("Cámaras disponibles")]
    [Tooltip("Asigna aquí todas tus CinemachineCameras en el orden que quieras (índice 0 = tecla 1, índice 1 = tecla 2, etc.)")]
    [SerializeField] private CinemachineCamera[] cameras; // En Cinemachine 3.x: CinemachineCamera (antes CinemachineVirtualCamera)

    [Header("Nombres para el log (opcional)")]
    [Tooltip("Nombres descriptivos para saber qué cámara es cuál en consola. Deben estar en el mismo orden que el arreglo de cameras.")]
    [SerializeField] private string[] cameraNames;

    [Header("Configuración de blend")]
    [Tooltip("Tiempo de transición entre cámaras en segundos. 0 = corte directo.")]
    [SerializeField] private float blendTime = 0f;

    private CinemachineBrain brain; // CinemachineBrain sigue existiendo en Cinemachine 3.x
    private int currentIndex = -1;

    void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Start()
    {
        if (cameras == null || cameras.Length == 0)
        {
            Debug.LogWarning("[CameraDirector] No hay cámaras asignadas.");
            return;
        }

        foreach (var cam in cameras)
            cam.Priority = 0;

        ActivateCamera(0);
    }

    void Update()
    {
        // Teclas 1–9 para saltar directo a esa cámara
        for (int i = 0; i < cameras.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                ActivateCamera(i);
        }

        // Flechas para ciclar
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ActivateCamera(Mathf.Max(0, currentIndex - 1));

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ActivateCamera(Mathf.Min(cameras.Length - 1, currentIndex + 1));
    }

    void ActivateCamera(int index)
    {
        if (index < 0 || index >= cameras.Length) return;
        if (index == currentIndex) return;

        // En Cinemachine 3.x el blend se configura directo en el DefaultBlend del Brain
        if (brain != null)
        {
            brain.DefaultBlend.Time = blendTime;
            brain.DefaultBlend.Style = blendTime > 0f
                ? CinemachineBlendDefinition.Styles.EaseInOut
                : CinemachineBlendDefinition.Styles.Cut;
        }

        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = (i == index) ? 20 : 0;

        currentIndex = index;

        string nombre = (cameraNames != null && index < cameraNames.Length && !string.IsNullOrEmpty(cameraNames[index]))
            ? cameraNames[index]
            : cameras[index].name;

        Debug.Log($"[CameraDirector] Cámara activa: [{index + 1}] {nombre}");
    }

    void OnGUI()
    {
        if (cameras == null || cameras.Length == 0) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 16;
        style.normal.textColor = Color.white;

        string nombre = (cameraNames != null && currentIndex >= 0 && currentIndex < cameraNames.Length && !string.IsNullOrEmpty(cameraNames[currentIndex]))
            ? cameraNames[currentIndex]
            : (currentIndex >= 0 ? cameras[currentIndex].name : "Ninguna");

        GUI.Box(new Rect(10, 10, 260, 30), $"CAM [{currentIndex + 1}]: {nombre}", style);
    }
}