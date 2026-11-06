using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text TextLabel;

    private void Start()
    {
        GetComponent<TypeWriterEffect>().Run("This is a bit of text!\n Helli", TextLabel);
    }

}
