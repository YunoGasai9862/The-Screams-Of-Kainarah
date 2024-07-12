using System.Collections;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private int DialogueCounter { get; set; } = 0;
    private SemaphoreSlim SemaphoreSlim { get; set;} =  new SemaphoreSlim(1);     // use this for dialogue, to make it not run fast/use ASYNC for each sentence

    private CancellationToken CancellationToken { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private bool test = true;

    [SerializeField]
    DialogueTriggerEvent dialogueTriggerEvent;

    private void Start()
    {
        dialogueTriggerEvent.AddListener(TriggerCoroutine);

        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;  
    }
    //bring back the displayNext functionality - we are doing that we a button click!!! - found the issue
    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        Debug.Log($"DialogueSystemReceived: {dialogueSystem.Dialogues.Count}");

        foreach (Dialogues dialogue in dialogueSystem.Dialogues)
        {
            Debug.Log("Initiating");

            if (!dialogueSystem.DialogueOptions.DialogueConcluded && !CancellationToken.IsCancellationRequested)
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
                    Debug.Log($"Count Before: {SemaphoreSlim.CurrentCount}");
                    _= SceneSingleton.GetDialogueManager().StartDialogue(SemaphoreSlim, CancellationTokenSource);
                    //pass semaphoreSlim to start dialogue
                    Debug.Log($"Count After: {SemaphoreSlim.CurrentCount}");
                    DialogueCounter++;
                }
            }
        }
    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        if (SemaphoreSlim.CurrentCount != 0 && test)
        {
            StartCoroutine(TriggerDialogue(dialogueSystem));
            test = false;
        }
    }
}
