using UnityEngine;

public class SingletonForDialogueManager : MonoBehaviour
{
    private static DialogueManager _dialogueManager;

    private void Awake()
    {
        _dialogueManager = GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>();
        Debug.Log(_dialogueManager);

    }
    public static DialogueManager getDialogueManager()
    {
        return _dialogueManager;
    }
}
