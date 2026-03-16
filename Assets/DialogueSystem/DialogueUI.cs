using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    PlayerController player;

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
        player = FindAnyObjectByType<PlayerController>();
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

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        // AÑADE ESTA LÍNEA AQUÍ:
        yield return null;

        Debug.Log("Iniciando secuencia de diálogo. Total frases: " + dialogueObject.Dialogue.Length);
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            Debug.Log("Mostrando frase índice: " + i);
            // ... resto del código
        }

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);
            textLabel.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            // --- 1. NUEVO: Esperar un frame para evitar que el mismo clic haga skip ---
            yield return null;

            // --- 2. NUEVO: Esperar hasta que el jugador SUELTE la tecla antes de volver a preguntar ---
            // Esto evita que si dejas el dedo pegado, se pasen todas las frases.
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space) && !Input.GetButton("Submit"));

            // 3. Ahora sí, esperamos a que la vuelva a presionar para la siguiente frase
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            if (player != null) player.canMove = true; // Asegúrate de que el player exista
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

