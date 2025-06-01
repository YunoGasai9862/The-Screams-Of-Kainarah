
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueSystem>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    [Header("Triggering Event")]
    [SerializeField] DialogueTriggerEvent dialogueTriggerEvent;

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

    public async void OnNotify(DialogueSystem data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        //make this the observer instead for the dialogue taking place
        //FIX THIS
        if (data.DialogueSettings.ShouldTriggerDialogue)
        {
            await TriggerDialogue(data);
        }
    }
}
