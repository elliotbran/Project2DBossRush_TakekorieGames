using UnityEngine;

public class IntroManager : MonoBehaviour
{

    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI; //Lets other scritps access the dialogueUI without allowing them to change it
    public IInteractable interactable { get; set; } //Gets the "IInteractable" interface from the object the player is interacting with

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable?.Interact(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
