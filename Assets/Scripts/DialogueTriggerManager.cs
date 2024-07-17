using System.Collections;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private int DialogueCounter { get; set; } = 0;
    private SemaphoreSlim SemaphoreSlim { get; set;} =  new SemaphoreSlim(1);     // use this for dialogue, to make it not run fast/use ASYNC for each sentence

    [SerializeField]
    public DialogueTriggerEvent dialogueTriggerEvent;
    public DialogueTakingPlaceEvent dialogueTakingPlaceEvent;

    private bool test = false;


    private void Start()
    {
        dialogueTriggerEvent.AddListener(TriggerCoroutine);
    }

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        dialogueTakingPlaceEvent.Invoke(true);

        foreach (Dialogues dialogue in dialogueSystem.Dialogues)
        {
            if (dialogueSystem.DialogueOptions.DialogueConcluded == false)
            {
                SceneSingleton.GetDialogueManager().PrepareDialogueQueue(dialogue);

                if (dialogueSystem.Dialogues.Count == DialogueCounter)
                {
                    Debug.Log("Concluded");
                    dialogueSystem.DialogueOptions.DialogueConcluded = true;

                    DialogueCounter = 0;

                    yield return null;
                }
                else
                {
                    SemaphoreSlim.Wait();

                    StartCoroutine(SceneSingleton.GetDialogueManager().StartDialogue(SemaphoreSlim));

                    DialogueCounter++;

                    Debug.Log(DialogueCounter);
                }
            }

            yield return new WaitUntil(() => SemaphoreSlim.CurrentCount > 0);

            Debug.Log("FINALLY");
        }
        
        if(SemaphoreSlim.CurrentCount > 0)
        {
            dialogueTakingPlaceEvent.Invoke(false);
        }
    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        Debug.Log(SceneSingleton.IsDialogueTakingPlace);
        if(SceneSingleton.IsDialogueTakingPlace == false && test == false)
        {
            test = true;
            Coroutine triggerDialogueCoroutine = StartCoroutine(TriggerDialogue(dialogueSystem));
        }
    }
}
