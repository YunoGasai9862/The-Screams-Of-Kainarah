
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using Annotations.Enums;
using UnityEngine;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(DialogueObserverManager), SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(DialogueObserverManager), SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(DialoguesAndOptions.DialogueSystem))]
public class DialogueObserverManager : MonoBehaviour, INotify<DialoguesAndOptions.DialogueSystem>, INotify<GenericStateBundle<GameStateBundle>>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    [Header("Triggering Event")]
    [SerializeField] DialogueTriggerEvent dialogueTriggerEvent;

    private Delegator Delegator { get; set; }

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private IEnumerator TriggerDialogue(DialoguesAndOptions.DialogueSystem dialogueSystem)
    {
        dialogueTriggerEvent.Invoke(dialogueSystem);

        yield return null;
    }

    private async void Start()
    {
       StartCoroutine(Helper.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<GenericStateBundle<GameStateBundle>> ()
        {
            Instance = gameObject,
            EntityType = typeof(DialogueObserverManager),
            SubjectType = typeof(GameStateConsumer)

        }, this);


        Delegator.NotifySubjectWrapper(new ObserverContext<DialoguesAndOptions.DialogueSystem>()
        {
            Instance = gameObject,
            EntityType = typeof(DialogueObserverManager),
            SubjectType = typeof(PlayerActionRelayer)

        }, this);

    }

    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        CurrentGameState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(DialoguesAndOptions.DialogueSystem value)
    {
        if (value.DialogueSettings.ShouldTriggerDialogue && !CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
           yield return StartCoroutine(TriggerDialogue(value));
        }
    }
}
