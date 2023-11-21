using NUnit.Framework;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Entities", menuName = "Dialogue Entities")]
[Serializable]
public class DialogueEntityScriptableObject: ScriptableObject
{

    [Serializable]
    public class DialogueEntity
    {
        public GameObject entity;
        public bool shouldDialogueTrigger;
        public bool multipleDialogues;
    }

    public DialogueEntity[] entities;
}
