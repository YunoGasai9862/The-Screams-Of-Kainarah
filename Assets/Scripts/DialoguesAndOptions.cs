using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues And Options", menuName = "Dialogue And Options")]
[Serializable]
public class DialoguesAndOptions: ScriptableObject
{
    [Serializable]
    public class DialogueExchange
    {
        public List<Dialogues> dialogues;
        public DialogueOptions dialogueOptions;
    }

    public List<DialogueExchange> exchange;
}