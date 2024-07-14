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
    //bring back the displayNext functionality - we are doing that we a button click!!! - found the issue
    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {

        foreach (Dialogues dialogue in dialogueSystem.Dialogues)
        {
            if (!dialogueSystem.DialogueOptions.DialogueConcluded)
            {
                SceneSingleton.GetDialogueManager().PrepareDialogueQueue(dialogue);

                if (dialogue.Sentences.Length == DialogueCounter)
                {
                    dialogueSystem.DialogueOptions.DialogueConcluded = true;

                    DialogueCounter = 0;

                    yield return null;
                }
                else
                {
                    SemaphoreSlim.Wait();

                    StartCoroutine(SceneSingleton.GetDialogueManager().StartDialogue(SemaphoreSlim));

                    DialogueCounter++;
                }
            }
        }
    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        Debug.Log($"Dialogue Taking Place {SceneSingleton.IsDialogueTakingPlace}");
        //ensures only onc
        if(SceneSingleton.IsDialogueTakingPlace == false)
            StartCoroutine(TriggerDialogue(dialogueSystem));
    }
}
