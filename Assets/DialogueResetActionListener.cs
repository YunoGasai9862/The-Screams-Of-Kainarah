using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueResetActionListener : MonoBehaviour, IObserver<DialoguesAndOptions>
{
    private void OnEnable()
    {
        PlayerObserverListenerHelper.DialogueAndOptions.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.DialogueAndOptions.RemoveOberver(this);
    }

    private Task ResetDialogueSystem(DialoguesAndOptions Data)
    {
        foreach(DialogueSystem dialogueSystem in Data.exchange)
        {
            dialogueSystem.DialogueOptions.DialogueConcluded = false;
        }

        return Task.CompletedTask;
    }

    public async void OnNotify(DialoguesAndOptions Data, params object[] optional)
    {
        if (Data != null)
        {
            await ResetDialogueSystem(Data);    
        }
    }

}
