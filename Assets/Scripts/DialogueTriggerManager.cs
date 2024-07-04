using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private static int m_dialogueCounter = 0;

    [SerializeField]
    public DialoguesAndOptions DialoguesAndOptions;

    public static IEnumerator TriggerDialogue(List<Dialogues> dialogues)
    {
        if (!Conversations.MultipleDialogues[dialogue].dialogueConcluded)
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
