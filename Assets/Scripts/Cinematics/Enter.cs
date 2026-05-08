using UnityEngine;
using UnityEngine.SceneManagement;

public class Enter : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string sceneName = "Kitchen"; 
    [SerializeField] private bool onlyInMenu = true;          

    private void Update()
    {
        // Detecta si se presiona Enter (o Return)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        // Evita cargar la misma escena en la que ya estás
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            Debug.LogWarning("Ya estás en la escena " + sceneName);
            return;
        }

        Debug.Log("Cargando escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}