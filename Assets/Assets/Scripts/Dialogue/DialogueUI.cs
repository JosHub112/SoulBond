using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text TextLabel;
    [SerializeField] private DialogueObject testDialogue;

    private ResponseHandler responseHandler;
    private TypeWriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypeWriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, TextLabel);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;
            yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponse(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        TextLabel.text = string.Empty;
    }
}