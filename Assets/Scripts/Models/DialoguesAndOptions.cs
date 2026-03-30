using Annotations.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

[Asset(Asset.SCRIPTABLE_OBJECT, "DialoguesAndOptions", InstantiationOrder = 4)]
[CreateAssetMenu(fileName = "Dialogues And Options", menuName = "Dialogue And Options")]
[Serializable]
public class DialoguesAndOptions: ScriptableObject
{
    [Serializable]
    public class DialogueSystem
    {
        [SerializeField]
        private List<DialogueSetup> _dialogueSetup;
        [SerializeField]
        private DialogueSettings _dialogueSettings;
        [SerializeField]
        private DialogueTriggeringEntity _dialogueTriggeringEntity;

        public List<DialogueSetup> DialogueSetup { get => _dialogueSetup; }
        public DialogueSettings DialogueSettings { get => _dialogueSettings;  }
        public DialogueTriggeringEntity DialogueTriggeringEntity { get => _dialogueTriggeringEntity; }
    }

    public List<DialogueSystem> exchange;
}