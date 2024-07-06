using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private static int m_dialogueCounter = 0;

    public static IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        if (!dialogueSystem.dialogueOptions.dialogueConcluded)
        {
            if (dialogue.Length == m_dialogueCounter)
            {
                Conversations.MultipleDialogues[dialogue].dialogueConcluded = true;
                m_dialogueCounter = 0;
                yield return null;

            }
            else
            {
                FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue[m_dialogueCounter], dialogue);
                m_dialogueCounter++;
            }

        }
    }
}
