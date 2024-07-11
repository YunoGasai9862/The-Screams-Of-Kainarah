using System.Collections;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private int DialogueCounter { get; set; } = 0;
    private SemaphoreSlim SemaphoreSlim { get; set;} =  new SemaphoreSlim(1);     // use this for dialogue, to make it not run fast/use ASYNC for each sentence

    [SerializeField]
    DialogueTriggerEvent dialogueTriggerEvent;

    private void Start()
    {
        dialogueTriggerEvent.AddListener(TriggerCoroutine);
    }

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {

        foreach (Dialogues dialogue in dialogueSystem.Dialogues)
        {
            Debug.Log("Initiating");

            if (!dialogueSystem.DialogueOptions.DialogueConcluded)
            {
                SceneSingleton.GetDialogueManager().PrepareDialogueQueue(dialogue);

                if (dialogue.Sentences.Length == DialogueCounter)
                {
                    //use event from dialogue Manager instead
                    dialogueSystem.DialogueOptions.DialogueConcluded = true;
                    DialogueCounter = 0;
                    yield return null;

                }
                else
                {
                    SemaphoreSlim.Wait();
                    SceneSingleton.GetDialogueManager().StartDialogue();
                    //pass semaphoreSlim to start dialogue
                    yield return new WaitUntil(() => SemaphoreSlim.CurrentCount > 0);
                    DialogueCounter++;
                }
            }
        }
        SemaphoreSlim.Release();
    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        if (SemaphoreSlim.CurrentCount != 0)
        {
            StartCoroutine(TriggerDialogue(dialogueSystem));
        }
    }
}
