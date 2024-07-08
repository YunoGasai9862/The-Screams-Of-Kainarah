using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static DialoguesAndOptions;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueSystem>
{
    [Header("Dialogues And Options")]
    [SerializeField] DialoguesAndOptions DialoguesAndOptions;

    Dictionary<string, Func<DialogueSystem, Task>> dialogueManagerActionDict; //Func -> the type of parameter it will take that is Dialogues,and the return type will be Task. It's c# delegation

    private void Start()
    {
        dialogueManagerActionDict = PrefilDictionaryFromTheScriptableObject(DialoguesAndOptions);
    }

    private Dictionary<string, Func<DialogueSystem, Task>> PrefilDictionaryFromTheScriptableObject(DialoguesAndOptions dialogueAndOptions)
    {
        var dictionary = new Dictionary<string, Func<DialogueSystem, Task>>();

        foreach (var item in dialogueAndOptions.exchange)
        {
            dictionary.Add(item.DialogueTriggeringEntity.Entity.tag, dialogues => TriggerDialogue(dialogues));
        }
        return dictionary;
    }
    private async Task TriggerDialogue(DialogueSystem dialogueSystem)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.5));

       StartCoroutine(DialogueTriggerManager.TriggerDialogue(dialogueSystem));
    }
    private void OnEnable()
    {
        PlayerObserverListenerHelper.DialogueSystem.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.DialogueSystem.RemoveOberver(this); 
    }

    public void OnNotify(DialogueSystem Data, params object[] optional)
    {
        if(dialogueManagerActionDict.TryGetValue(Data.DialogueTriggeringEntity.EntityTag, out var func))
        {
            if (Data.DialogueOptions.ShouldTriggerDialogue)
            {
                func.Invoke(Data);  //solved it!! casting is needed to cast it to dialogues as in the dictionary its object
            }
        }
    }

}
