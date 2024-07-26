using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task PreloadAudio(DialoguesAndOptions dialogueAndOptions)
    {
        
        foreach(Dialogues dialogues in await ExtractTextAudioPaths(dialogueAndOptions))
        {
            foreach(TextAudioPath textAudio in dialogues.TextAudioPath)
            {
                //generate Audio Name etc look later
                await awsPollyDialogueTriggerEvent.Invoke(new AWSPollyAudioPacket { AudioName = "", AudioVoiceId = dialogues.VoiceID, DialogueText = textAudio.Sentence });
                //once generated, write back the audio path here
            }
        }
    }

    private Task<List<Dialogues>> ExtractTextAudioPaths(DialoguesAndOptions dialoguesAndOptions)
    {
        List<TextAudioPath> textAudioPath = new List<TextAudioPath>();
        TaskCompletionSource<List<Dialogues>> tcs = new TaskCompletionSource<List<Dialogues>>();   
        Task.Run(() =>
        {
            tcs.SetResult(dialoguesAndOptions.exchange.
               SelectMany(dialogues => dialogues.Dialogues).ToList());

        });

        return tcs.Task;
    }
}