using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialoguesAndOptions>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    Dictionary<string, Func<List<Dialogues>, Task>> dialogueManagerActionDict; //Func -> the type of parameter it will take that is Dialogues,and the return type will be Task. It's c# delegation

    private void Start()
    {
        dialogueManagerActionDict = PrefilDictionaryFromTheScriptableObject(DialoguesAndOptions);
    }

    private Dictionary<string, Func<List<Dialogues>, Task>> PrefilDictionaryFromTheScriptableObject(DialoguesAndOptions dialogueAndOptions)
    {
        var dictionary = new Dictionary<string, Func<List<Dialogues>, Task>>();

        foreach (var item in dialogueAndOptions.exchange)
        {
            dictionary.Add(item.dialogueTriggeringEntity.triggeringEntity.tag, dialogues => TriggerDialogue(dialogues));
        }
        return dictionary;
    }
    private async Task TriggerDialogue(List<Dialogues> dialogues)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.5));

       StartCoroutine(DialogueTriggerManager.TriggerDialogue(dialogues));
    }
    private void OnEnable()
    {
        PlayerObserverListenerHelper.DialogueEntites.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.DialogueEntites.RemoveOberver(this); 
    }

    public void OnNotify(DialoguesAndOptions Data, params object[] optional)
    {
        if(dialogueManagerActionDict.TryGetValue(Data.entity.tag, out var func))
        {
            if (Data.shouldDialogueTrigger)
            {
              //  List<Dialogues> dialogues = DialoguesAndOptions.DialogueExchange

                func.Invoke(dialogues);  //solved it!! casting is needed to cast it to dialogues as in the dictionary its object
            }
        }
    }

}
