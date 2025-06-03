
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueSystem>, IObserver<GameState>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    [Header("Triggering Event")]
    [SerializeField] DialogueTriggerEvent dialogueTriggerEvent;

    [Header("Triggering Event")]
    [SerializeField] GlobalGameStateDelegator globalGameStateDelegator;

    private GameState GameState { get; set; }

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
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None);
    }

    public async void OnNotify(DialogueSystem data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (data.DialogueSettings.ShouldTriggerDialogue && !GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            await TriggerDialogue(data);
        }
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameState = data;
    }
}
