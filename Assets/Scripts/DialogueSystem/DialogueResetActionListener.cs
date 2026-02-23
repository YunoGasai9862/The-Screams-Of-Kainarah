using Assets.Annotations;
using System.Collections;
using System.Threading;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Threading.Tasks;
using UnityEngine;

[Observer(ObserverType = typeof(DialogueResetActionListener), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
public class DialogueResetActionListener : MonoBehaviour, INotify<EntityPoolManager>
{
    private const string DIALOGUES_AND_OPTIONS_KEY = "DialoguesAndOptions";

    private DialoguesAndOptions DialoguesAndOptionsSO { get; set; }

    private EntityPoolManager EntityPoolManagerInstance { get; set; }

    private async void OnDisable()
    {
        await ResetDialogueSystem(DialoguesAndOptionsSO);
    }

    private Task ResetDialogueSystem(DialoguesAndOptions Data)
    {
        foreach(DialogueSystem dialogueSystem in Data.exchange)
        {
            dialogueSystem.DialogueSettings.DialogueConcluded = false;
        }

        return Task.CompletedTask;
    }

    public async void OnNotify(DialoguesAndOptions data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (data != null)
        {
            await ResetDialogueSystem(data);
        }
    }

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        DialoguesAndOptionsSO = Helper.GetFromEntityPoolManager<DialoguesAndOptions>(EntityPoolManagerInstance, DIALOGUES_AND_OPTIONS_KEY);

        yield return null;
    }
}
