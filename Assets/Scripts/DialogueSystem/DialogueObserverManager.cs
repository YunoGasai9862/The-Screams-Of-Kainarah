
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using UnityEngine;
using static DialoguesAndOptions;

[Observer(ObserverType = typeof(DialogueObserverManager), SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
[Observer(ObserverType = typeof(DialogueObserverManager), SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(DialogueSystem))]
public class DialogueObserverManager : MonoBehaviour, INotify<DialogueSystem>, INotify<GenericStateBundle<GameStateBundle>>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    [Header("Triggering Event")]
    [SerializeField] DialogueTriggerEvent dialogueTriggerEvent;

    private Delegator Delegator { get; set; }

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private IEnumerator TriggerDialogue(DialogueSystem dialogueSystem)
    {
        dialogueTriggerEvent.Invoke(dialogueSystem);

        yield return null;
    }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        Delegator.NotifySubjectWrapper(new ObserverContext<GenericStateBundle<GameStateBundle>> ()
        {
            Instance = gameObject,
            EntityType = typeof(DialogueObserverManager),
            SubjectType = typeof(GameStateConsumer)

        }, this);


        Delegator.NotifySubjectWrapper(new ObserverContext<DialogueSystem>()
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

    public IEnumerator Notify(DialogueSystem value)
    {
        if (value.DialogueSettings.ShouldTriggerDialogue && !CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
           yield return StartCoroutine(TriggerDialogue(value));
        }
    }
}
