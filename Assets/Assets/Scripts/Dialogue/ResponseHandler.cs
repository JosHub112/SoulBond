using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform requestButtonTmplate;
    [SerializeField] private RectTransform responseContainer;

    private DialogueUI dialogueUI;

    private List<GameObject> tempResponses = new List<GameObject>();

    private void Start()
    {
       dialogueUI=GetComponent<DialogueUI>();
    }

    public void ShowResponse(Response[] responses)
    {
        float responseBoxHeight = 0;

        foreach (Response response in responses)
        {
            GameObject responseButton = Instantiate(requestButtonTmplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponentInChildren<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));

            tempResponses.Add(responseButton);

            responseBoxHeight += requestButtonTmplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, y: responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response)
    {
        responseBox.gameObject.SetActive (false);

        foreach (GameObject button in tempResponses)
        {
            Destroy(button);
        }

        dialogueUI.ShowDialogue(response.DialogueObject);
    }
}