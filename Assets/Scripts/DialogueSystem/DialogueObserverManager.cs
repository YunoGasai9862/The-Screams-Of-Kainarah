
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueSystem>, IObserver<GenericState<GameState>>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    [Header("Triggering Event")]
    [SerializeField] DialogueTriggerEvent dialogueTriggerEvent;

    [Header("Triggering Event")]
    [SerializeField] GlobalGameStateDelegator globalGameStateDelegator;

    private GenericState<GameState> CurrentGameState { get; set; } = new GenericState<GameState>();

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

    private void Start()
    {
        globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GameStateConsumer).ToString()

        }, CancellationToken.None);

    }

    public async void OnNotify(DialogueSystem data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (data.DialogueSettings.ShouldTriggerDialogue && !CurrentGameState.State.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            await TriggerDialogue(data);
        }
    }

    public void OnNotify(GenericState<GameState> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState.State = data.State;
    }
}
