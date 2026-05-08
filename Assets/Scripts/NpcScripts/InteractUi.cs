using UnityEngine;

public class InteractUi : MonoBehaviour
{
    [Header("UI")]
    public GameObject pressEImage;

    [Header("Objeto que aparecerá")]
    public GameObject objectToShow;

    private bool playerInside = false;

    private void Start()
    {
        if (pressEImage != null)
            pressEImage.SetActive(false);

        if (objectToShow != null)
            objectToShow.SetActive(false);
    }

    private void Update()
    {
        if (playerInside &&
            Input.GetKeyDown(KeyCode.E))
        {
            if (objectToShow != null)
                objectToShow.SetActive(true);

            if (pressEImage != null)
                pressEImage.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (pressEImage != null)
                pressEImage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (pressEImage != null)
                pressEImage.SetActive(false);
        }
    }
}