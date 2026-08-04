using Assets.Annotations;
using System.Collections;
using System.Threading;
using UnityEngine;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(DialogueTriggerManager), SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(DialogueTriggerManager), SubjectType = typeof(DialogueManager), ContextType = typeof(DialogueManager))]
public class DialogueTriggerManager : MonoBehaviorScene, INotify<GenericStateBundle<GameStateBundle>>, INotify<DialogueManager>
{
    [SerializeField]
    public DialogueTriggerEvent dialogueTriggerEvent;
    [SerializeField]
    public GameStateEvent gameStateEvent;

    private Delegator Delegator { get; set; }

    private DialogueManager DialogueManager { get; set; }
    private int DialogueCounter { get; set; } = 0;
    private GenericStateBundle<GameStateBundle> GameStateBundle { get; set; } = new GenericStateBundle<GameStateBundle>();
    private SemaphoreSlim SemaphoreSlim { get; set; } = new SemaphoreSlim(1);

    private async void Start()
    {
        await dialogueTriggerEvent.AddListener(TriggerCoroutine);

       StartCoroutine((await (await GetBaseScene()).GetSceneUtilsAsync()).GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<GenericStateBundle<GameStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(DialogueTriggerManager),
            SubjectType = typeof(GameStateConsumer)

        }, this);

        Delegator.NotifySubjectWrapper(new ObserverContext<DialogueManager>()
        {
            Instance = gameObject,
            EntityType = typeof(DialogueTriggerManager),
            SubjectType = typeof(DialogueManager)

        }, this);
    }

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        yield return new WaitUntil(() => DialogueManager != null);

        BroadcastGameState(new State<GameState>() { CurrentState = GameState.DIALOGUE_TAKING_PLACE, IsConcluded = false });

        foreach (DialogueSetup dialogue in dialogueSystem.DialogueSetup)
        {
            DialogueManager.PrepareDialoguesQueue(dialogue);

            SemaphoreSlim.Wait();

            StartCoroutine(DialogueManager.StartDialogue(SemaphoreSlim));

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

    public void OnNotify(GenericStateBundle<GameStateBundle> data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
    }

    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        GameStateBundle = value;

        yield return null;
    }

    public IEnumerator Notify(DialogueManager value)
    {
        DialogueManager = value;

        yield return null;
    }
}
