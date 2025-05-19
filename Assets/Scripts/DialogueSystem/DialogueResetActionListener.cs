using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueResetActionListener : MonoBehaviour, IObserver<DialoguesAndOptions>
{
    private void OnEnable()
    {
        ResetNotifierSubjects.DialogueAndOptions.AddObserver(this);
    }

    private void OnDisable()
    {
        ResetNotifierSubjects.DialogueAndOptions.RemoveOberver(this);
    }

    private Task ResetDialogueSystem(DialoguesAndOptions Data)
    {
        foreach(DialogueSystem dialogueSystem in Data.exchange)
        {
            dialogueSystem.DialogueOptions.DialogueConcluded = false;
        }

        return Task.CompletedTask;
    }

    public async void OnNotify(DialoguesAndOptions data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (data != null)
        {
            await ResetDialogueSystem(data);
        }
    }
}
