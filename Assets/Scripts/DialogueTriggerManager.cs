using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private static int m_dialogueCounter = 0;

    public static IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        foreach(Dialogues dialogue in dialogueSystem.dialogues)
        {
            if (!dialogueSystem.dialogueOptions.dialogueConcluded)
            {
                SceneSingleton.GetDialogueManager().PrepareDialogueQueue(dialogue);

                if (dialogue.sentences.Length == m_dialogueCounter)
                {
                    //use event from dialogue Manager instead
                    dialogueSystem.dialogueOptions.dialogueConcluded = true;
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
