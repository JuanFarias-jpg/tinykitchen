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
        if (pressEImage != null)
            pressEImage.SetActive(false);

        if (pressPImage != null)
            pressPImage.SetActive(false);
    }

    private void Update()
    {
        DetectNPC();

        bool eOnCooldown = Time.time < nextInteractTime;

        
        if (pressEImage != null)
        {
            bool canShowE =
                currentNpc != null &&
                !DialogueUI.Instance.IsOpen() &&
                !eOnCooldown;

            pressEImage.SetActive(canShowE);
        }

        if (pressPImage != null)
        {
            bool canShowP =
                currentNpc != null &&
                !DialogueUI.Instance.IsOpen();

            pressPImage.SetActive(canShowP);
        }

      

        if (!eOnCooldown &&
            Input.GetKeyDown(KeyCode.E) &&
            currentNpc != null &&
            !DialogueUI.Instance.IsOpen())
        {
            if (pressEImage != null)
                pressEImage.SetActive(false);

            currentNpc.Interact(transform);

            nextInteractTime = Time.time + interactCooldown;
        }

       
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
    }
}