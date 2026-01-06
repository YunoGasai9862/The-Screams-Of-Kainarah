
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueSystem>, IObserver<GenericStateBundle<GameStateBundle>>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    [Header("Triggering Event")]
    [SerializeField] DialogueTriggerEvent dialogueTriggerEvent;

    private GlobalGameStateDelegator GlobalGameStateDelegator {get; set; }

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private async Task TriggerDialogue(DialogueSystem dialogueSystem)
    {
        await dialogueTriggerEvent.Invoke(dialogueSystem);
    }
    private void OnEnable()
    {
        PlayerObserverListenerHelper.DialogueSystem.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.DialogueSystem.RemoveOberver(this); 
    }

    private async void Start()
    {
        GlobalGameStateDelegator = await Helper.GetDelegator<GlobalGameStateDelegator>();

        GlobalGameStateDelegator.NotifySubjectWrapper(this, new ObserverContext()
        {
            Name = this.name,
            Tag = this.name,
            SubjectType = typeof(GameStateConsumer)

        }, CancellationToken.None);

    }

    public async void OnNotify(DialogueSystem data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (data.DialogueSettings.ShouldTriggerDialogue && !CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            await TriggerDialogue(data);
        }
    }

    public void OnNotify(GenericStateBundle<GameStateBundle> data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState.StateBundle = data.StateBundle;
    }
}
