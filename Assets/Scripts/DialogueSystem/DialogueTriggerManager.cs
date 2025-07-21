using System.Collections;
using System.Threading;
using UnityEngine;

public class DialogueTriggerManager : MonoBehaviour, IObserver<GenericStateBundle<GameStateBundle>>
{
    private int DialogueCounter { get; set; } = 0;
    private GenericStateBundle<GameStateBundle> GameStateBundle { get; set; } = new GenericStateBundle<GameStateBundle>();
    private SemaphoreSlim SemaphoreSlim { get; set;} =  new SemaphoreSlim(1);

    [SerializeField]
    public DialogueTriggerEvent dialogueTriggerEvent;
    [SerializeField]
    public GameStateEvent gameStateEvent;
    [SerializeField]
    GlobalGameStateDelegator globalGameStateDelegator;

    private void Start()
    {
        dialogueTriggerEvent.AddListener(TriggerCoroutine);

        globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GameStateConsumer).ToString()

        }, CancellationToken.None);
    }

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        BroadcastGameState(new State<GameState>() { CurrentState = GameState.DIALOGUE_TAKING_PLACE, IsConcluded = false });

        foreach (DialogueSetup dialogue in dialogueSystem.DialogueSetup)
        {
            SceneSingleton.GetDialogueManager().PrepareDialoguesQueue(dialogue);

            SemaphoreSlim.Wait();

            StartCoroutine(SceneSingleton.GetDialogueManager().StartDialogue(SemaphoreSlim));

            DialogueCounter++;

            yield return new WaitUntil(() => SemaphoreSlim.CurrentCount > 0);

            if (dialogueSystem.DialogueSetup.Count == DialogueCounter)
            {
                dialogueSystem.DialogueSettings.DialogueConcluded = true;

                DialogueCounter = 0;

                BroadcastGameState(new State<GameState>() { CurrentState = GameState.FREE_MOVEMENT, IsConcluded = false });

                yield return null;
            }
        }
    }

    public void TriggerCoroutine(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        if (GameStateBundle.StateBundle.GameState.CurrentState == GameState.DIALOGUE_TAKING_PLACE || dialogueSystem.DialogueSettings.DialogueConcluded)
        {
            return;
        }

        Coroutine triggerDialogueCoroutine = StartCoroutine(TriggerDialogue(dialogueSystem));
    }

    private async void BroadcastGameState(State<GameState> gameState)
    {
        GameStateBundle.StateBundle.GameState = gameState;

        await gameStateEvent.Invoke(GameStateBundle);
    }

    public void OnNotify(GenericStateBundle<GameStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameStateBundle = data;
    }
}
