using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private static int m_dialogueCounter = 0;

    private SemaphoreSlim m_semaphoreSlim = new SemaphoreSlim(1); 
    // use this for dialogue, to make it not run fast/use ASYNC for each sentence

    public static IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        foreach(Dialogues dialogue in dialogueSystem.Dialogues)
        {
            Debug.Log("Initiating");

            if (!dialogueSystem.DialogueOptions.DialogueConcluded)
            {
                SceneSingleton.GetDialogueManager().PrepareDialogueQueue(dialogue);

                if (dialogue.Sentences.Length == m_dialogueCounter)
                {
                    //use event from dialogue Manager instead
                    dialogueSystem.DialogueOptions.DialogueConcluded = true;
                    m_dialogueCounter = 0;
                    yield return null;

                }
                else
                {
                    SceneSingleton.GetDialogueManager().StartDialogue();
                    m_dialogueCounter++;
                }

            }
        }
    }
}
