using System.Collections;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private int DialogueCounter { get; set; } = 0;
    private SemaphoreSlim SemaphoreSlim { get; set;} =  new SemaphoreSlim(1);

    [SerializeField]
    public DialogueTriggerEvent dialogueTriggerEvent;
    public DialogueTakingPlaceEvent dialogueTakingPlaceEvent;

    private void Start()
    {
        dialogueTriggerEvent.AddListener(TriggerCoroutine);
    }

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        dialogueTakingPlaceEvent.Invoke(true);

        foreach (Dialogues dialogue in dialogueSystem.Dialogues)
        {
            SceneSingleton.GetDialogueManager().PrepareDialoguesQueue(dialogue);

            SemaphoreSlim.Wait();

            StartCoroutine(SceneSingleton.GetDialogueManager().StartDialogue(SemaphoreSlim));

            DialogueCounter++;

            yield return new WaitUntil(() => SemaphoreSlim.CurrentCount > 0);

            if (dialogueSystem.Dialogues.Count == DialogueCounter)
            {
                dialogueSystem.DialogueOptions.DialogueConcluded = true;

                DialogueCounter = 0;

                yield return null;
            }
        }

        dialogueTakingPlaceEvent.Invoke(false);

    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        if(!SceneSingleton.IsDialogueTakingPlace && !dialogueSystem.DialogueOptions.DialogueConcluded)
        {
            Coroutine triggerDialogueCoroutine = StartCoroutine(TriggerDialogue(dialogueSystem));
        }
    }
}
