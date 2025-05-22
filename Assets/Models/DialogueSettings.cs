using System;
using UnityEngine;
[Serializable]
public class DialogueSettings
{
    [SerializeField]
    private bool _dialogueConcluded;
    [SerializeField]
    private bool _multipleDialogues;
    [SerializeField]
    private bool _shouldTriggerDialogue;

    public bool DialogueConcluded { get => _dialogueConcluded; set => _dialogueConcluded = value; }
    public bool MultipleDialogues { get => _multipleDialogues; set => _multipleDialogues = value; }
    public bool ShouldTriggerDialogue { get => _shouldTriggerDialogue; set => _shouldTriggerDialogue = value; }
}