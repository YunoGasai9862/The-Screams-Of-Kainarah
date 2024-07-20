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
        return Task.CompletedTask;
    }
}