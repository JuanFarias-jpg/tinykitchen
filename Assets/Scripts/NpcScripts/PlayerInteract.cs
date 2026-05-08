// PlayerInteract.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private float interactCooldown = 0.55f;
    
    [Header("UI")]
    [SerializeField] private GameObject pressEImage;
    [SerializeField] private GameObject pressPImage;

    private float nextInteractTime;
    private NPCInteractable currentNpc;

    private void Start()
    {
        pressEImage.SetActive(false);
        pressPImage.SetActive(false);
    }

    private void Update()
    {
        DetectNPC();

        if (Time.time < nextInteractTime)
            return;

        // Interactuar con E
        if (Input.GetKeyDown(KeyCode.E) &&
            currentNpc != null &&
            !DialogueUI.Instance.IsOpen())
        {
            pressEImage.SetActive(false);
            pressPImage.SetActive(false);
           
            currentNpc.Interact(transform);

            nextInteractTime = Time.time + interactCooldown;
        }

        // Ir a tienda con P
        if (Input.GetKeyDown(KeyCode.P) &&
            currentNpc != null &&
            !DialogueUI.Instance.IsOpen())
        {
            SceneManager.LoadScene("Personalizacion");
        }
    }

    private void DetectNPC()
    {
        currentNpc = null;

        Collider[] colliderArray =
            Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out NPCInteractable npc))
            {
                currentNpc = npc;
                break;
            }
        }

        bool inRange = currentNpc != null &&
                       !DialogueUI.Instance.IsOpen();

        pressEImage.SetActive(inRange);
        pressPImage.SetActive(inRange);
    }
}