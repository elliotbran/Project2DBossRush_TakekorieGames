using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    PlayerController playerController;
    [SerializeField] private bool isTriggerPlayer;
   // [SerializeField] private bool isTriggerNPC;

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggerPlayer)
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.canMove = false;
                //playerController.interactable = this;
                playerController.interactable?.Interact(playerController); // Cambiado 'this' por 'playerController'
            }
        }

        if(other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.interactable = this;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            if (player.interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.interactable = null;
            }
        }
    }
    public void Interact(PlayerController player)
    {
        foreach(DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if(responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
    }
}
