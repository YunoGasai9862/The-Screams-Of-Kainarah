using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues And Options", menuName = "Dialogue And Options")]
[Serializable]
public class DialoguesAndOptions: ScriptableObject
{
    public Dialogues dialogues;
    public DialogueOptions dialogueOptions;
}