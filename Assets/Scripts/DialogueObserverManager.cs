using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static DialogueEntityScriptableObject;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueEntity>
{

    Dictionary<string, Func<Dialogues[], Task>> dialogueManagerActionDict; //Func -> the type of parameter it will take that is Dialogues,and the return type will be Task. It's c# delegation

    private void Start()
    {
        dialogueManagerActionDict = new Dictionary<string, Func<Dialogues[], Task>>
        {
            { "Boss",   dialogues => TriggerDialogue(dialogues) },
            { "Vendor", dialogues => TriggerDialogue(dialogues) }
        };
    }

    private async Task TriggerDialogue(Dialogues[] dialogues)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        Debug.Log(dialogues.Length);
        StartCoroutine(Interactable.TriggerDialogue(dialogues));
     
    }
    private void OnEnable()
    {
        PlayerObserverListenerHelper.DialogueEntites.AddObserver(this);

    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.DialogueEntites.RemoveOberver(this); 
    }
    public void OnNotify(ref DialogueEntity Data, params object[] optional)
    {
        if(dialogueManagerActionDict.TryGetValue(Data.entity.tag, out var func))
        {
            if(Data.shouldDialogueTrigger)
            {
                func.Invoke((Dialogues[])Interactable.GetDialoguesDict[Data.entity.tag]);  //solved it!! casting is needed to cast it to dialogues as in the dictionary its object
            }
        }
    }

}
