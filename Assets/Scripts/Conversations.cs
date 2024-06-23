using Amazon.Polly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversations : MonoBehaviour
{
    public Dialogues dialogue;
    public Dialogues bossDialogue;

    private static Dictionary<string, object> dialoguesDictionary; //can be list or singular


    [Header("Conversation between Vendor and the Player")]
    public Dialogues[] wizardPlayerConvo;

    //dictionaries
    public static Dictionary<Dialogues, DialogueOptions> dialogueDictionary;
    public static Dictionary<Dialogues[], DialogueOptions> MultipleDialogues;

    //getter
    public static Dictionary<string, object> GetDialoguesDict { get => dialoguesDictionary; }

    //use scriptable object to keep dialogues now :) - more cleaner with Dialogues and their options -> refactoring

    private void Start()
    {
        dialogueDictionary = new Dictionary<Dialogues, DialogueOptions>();
        MultipleDialogues = new Dictionary<Dialogues[], DialogueOptions>();

        dialogueDictionary.Add(dialogue, new DialogueOptions { dialogueConcluded = false,  multipleDialogues = false, voiceId = VoiceId.Emma });
        dialogueDictionary.Add(bossDialogue, new DialogueOptions { dialogueConcluded = false, multipleDialogues = false, voiceId = VoiceId.Enrique});
        MultipleDialogues.Add(wizardPlayerConvo, new DialogueOptions { dialogueConcluded = false, multipleDialogues = true, voiceId = VoiceId.Jacek});

        dialoguesDictionary = new Dictionary<string, object>
        {
            {"TriggerPoint1", dialogue},
            {"Boss",  bossDialogue},
            {"Vendor", wizardPlayerConvo}
        };

    }
}
