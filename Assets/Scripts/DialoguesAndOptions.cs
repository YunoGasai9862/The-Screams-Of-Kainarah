using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues And Options", menuName = "Dialogue And Options")]
[Serializable]
public class DialoguesAndOptions: ScriptableObject
{
    [Serializable]
    public class DialogueExchange
    {
        public Dialogues dialogues;
        public Dialogues[] multiDialogues;
        public DialogueOptions dialogueOptions;
    }

    public DialogueExchange[] exchange;
}