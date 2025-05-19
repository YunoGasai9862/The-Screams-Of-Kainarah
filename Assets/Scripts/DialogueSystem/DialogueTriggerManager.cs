using System.Collections;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour
{
    private int DialogueCounter { get; set; } = 0;
    private GameState GameState { get; set; }
    private SemaphoreSlim SemaphoreSlim { get; set;} =  new SemaphoreSlim(1);

    [SerializeField]
    public DialogueTriggerEvent dialogueTriggerEvent;
    [SerializeField]
    public GameStateEvent gameStateEvent;

    private void Start()
    {
        dialogueTriggerEvent.AddListener(TriggerCoroutine);
    }

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        SetGameStateAndBroadcast(GameState.DIALOGUE_TAKING_PLACE);

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

        SetGameStateAndBroadcast(GameState.DIALOGUE_TAKING_PLACE);

    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        if(GameState != GameState.DIALOGUE_TAKING_PLACE && !dialogueSystem.DialogueOptions.DialogueConcluded)
        {
            Coroutine triggerDialogueCoroutine = StartCoroutine(TriggerDialogue(dialogueSystem));
        }
    }

    private void SetGameStateAndBroadcast(GameState value)
    {
        GameState = value;

        gameStateEvent.Invoke(value);
    }
}
