using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogues dialogue;
    public Dialogues BossDialogue;

    [Header("Conversation between Vendor and the Player")]
    public Dialogues[] WizardPlayerConvo;

    //dictionaries
    public static Dictionary<Dialogues, bool> dialogueDictionary;
    public static Dictionary<Dialogues[], bool> MultipleDialogues;



    public static int Dialoguecounter = 0;

    private void Start()
    {
        dialogueDictionary = new Dictionary<Dialogues, bool>();
        MultipleDialogues = new Dictionary<Dialogues[], bool>();
        dialogueDictionary.Add(dialogue, false);
        dialogueDictionary.Add(BossDialogue, false);
        MultipleDialogues.Add(WizardPlayerConvo, false);




        StartCoroutine(TriggerDialogue(dialogue));//because queue is already empty, thats why using Invoke to give some time to the queue
    }

    public IEnumerator TriggerDialogue(Dialogues dialogue)
    {
        yield return new WaitForSeconds(.1f);
        if (dialogueDictionary[dialogue] == false)
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
            dialogueDictionary[dialogue] = true; //already played
        }

    }


    public IEnumerator TriggerDialogue(Dialogues[] dialogue)
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
