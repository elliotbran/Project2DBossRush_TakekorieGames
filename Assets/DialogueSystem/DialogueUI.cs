using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject nameBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private TMP_Text charName;
    [SerializeField] private Image portraitImage;


    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        if(dialogueObject.hasName == true)
        {
            nameBox.SetActive(true);
        }
        else
        {
            nameBox.SetActive(false);
        }

        if (dialogueObject.character != null)
        {
            charName.text = dialogueObject.character.characterName;
        }

        if (dialogueObject.character != null && portraitImage != null)
        {
            portraitImage.sprite =
                dialogueObject.character.GetPortrait(dialogueObject.portraitId);

            portraitImage.gameObject.SetActive(true);
        }
        else if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(false);
        }


        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private  IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for(int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            if (i == dialogueObject.Responses.Length - 1 && dialogueObject.HasResponses) break;
            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                typewriterEffect.Stop();
            }
        }
    }   

    public void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}

