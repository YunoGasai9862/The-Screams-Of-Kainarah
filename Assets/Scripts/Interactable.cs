using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogues dialogue;
    public Dialogues bossDialogue;

    private static Dictionary<string, object> dialoguesDictionary; //can be list or singular


    [Header("Conversation between Vendor and the Player")]
    public Dialogues[] wizardPlayerConvo;

    //dictionaries
    public static Dictionary<Dialogues, bool> dialogueDictionary;
    public static Dictionary<Dialogues[], bool> MultipleDialogues;

    public static int Dialoguecounter = 0;

    //getter
    public static Dictionary<string, object> GetDialoguesDict { get => dialoguesDictionary; }

    private void Start()
    {
        dialogueDictionary = new Dictionary<Dialogues, bool>();
        MultipleDialogues = new Dictionary<Dialogues[], bool>();
        dialogueDictionary.Add(dialogue, false);
        dialogueDictionary.Add(bossDialogue, false);
        MultipleDialogues.Add(wizardPlayerConvo, false);

        dialoguesDictionary = new Dictionary<string, object>
        {
            {"TriggerPoint1", dialogue},
            {"Boss",  bossDialogue},
            {"Vendor", wizardPlayerConvo}
        };

    }

    public static IEnumerator TriggerDialogue(Dialogues dialogue)
    {
        yield return new WaitForSeconds(.1f);
        if (dialogueDictionary[dialogue] == false)
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
            dialogueDictionary[dialogue] = true; //already played
        }

    }


    public static IEnumerator TriggerDialogue(Dialogues[] dialogue)
    {
        if (MultipleDialogues[dialogue] == false)
        {
            if (dialogue.Length == Dialoguecounter)
            {
                MultipleDialogues[dialogue] = true;
                Dialoguecounter = 0;
                yield return null;

            }
            else
            {

                FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue[Dialoguecounter], dialogue);
                Dialoguecounter++;
            }

        }
    }
}
