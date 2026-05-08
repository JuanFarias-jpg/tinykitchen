using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public Sprite npcSprite;
    public Sprite mechanicSprite;

    public AudioSource audio;

    public PlayerInputHandler playerInputHandler;
    public MonoBehaviour playerMovement; 

    private Animator animator;
    private NPCHeadLookAt head;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        head = GetComponent<NPCHeadLookAt>();
    }

    public void Interact(Transform player)
    {
        // BLOQUEAR CONTROLES
        if (playerInputHandler != null)
            playerInputHandler.enabled = false;

        if (playerMovement != null)
            playerMovement.enabled = false;

        animator.SetTrigger("Talk");
        head.LookAtTarget(player);

        if (audio != null)
            audio.Play();

        DialogueUI.Instance.ShowDialogue(
        "Hey Chef! Welcome to Tiny Kitchen! Use WASD to move, Space to jump and double Space to Dive. Collect Yellow Cheese to buy skins with me, and White Cheese fully restores your health",
        npcSprite,
        mechanicSprite,

        () =>
        {
            if (audio != null)
                audio.Stop();

            animator.ResetTrigger("Talk");
            head.StopLooking();

            // DESBLOQUEAR CONTROLES
            if (playerInputHandler != null)
                playerInputHandler.enabled = true;

            if (playerMovement != null)
                playerMovement.enabled = true;
        });
    }
}