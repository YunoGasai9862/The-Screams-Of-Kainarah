using System;
using System.Threading.Tasks;
using UnityEngine;
public class AudioPreload: MonoBehaviour, IPreloadAudio<DialoguesAndOptions>
{
    [SerializeField]
    AWSPollyDialogueTriggerEvent awsPollyDialogueTriggerEvent;
    DialoguesAndOptions dialogueAndOptions;

    private async void Start()
    {
        //Do this during preloadign screen - another class for that already (GameLoad.cs) with loading UI
        await PreloadAudio(dialogueAndOptions);
    }
    public Task PreloadAudio(DialoguesAndOptions dialogueAndOptions)
    {
        foreach(DialoguesAndOptions.DialogueSystem dialogueSystem in dialogueAndOptions.exchange)
        {
            foreach(Dialogues dialogues in dialogueSystem.Dialogues)
            {
                //find a better way to not iterate through all those for loops???? any way to expose just all those textAudioPaths?
            }

           // awsPollyDialogueTriggerEvent.Invoke()
        }

        return Task.CompletedTask;
    }
}