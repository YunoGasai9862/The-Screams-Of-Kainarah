
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class AudioPreload : EntityPreloadMonoBehavior, IPreloadAudio<DialoguesAndOptions>
{
    private string PersistencePath { get; set; }

    private bool AudioGenerated { get; set; } = false;

    [SerializeField]
    AWSPollyDialogueTriggerEvent awsPollyDialogueTriggerEvent;
    [SerializeField]
    DialoguesAndOptions dialogueAndOptions;
    [SerializeField]
    AudioGeneratedEvent audioGeneratedEvent;

    private void Awake()
    {
        PersistencePath = Application.persistentDataPath;
    }

    private void Start()
    {
        //Do this during preloadign screen - another class for that already (GameLoad.cs) with loading UI
        //Here's the time issue! The game object is not active, look at tomorrow!!
         audioGeneratedEvent.AddListener(AudioGeneratedListener);
    }
    public IEnumerator PreloadAudio(DialoguesAndOptions dialogueAndOptions)
    {
        Task<List<Dialogues>> extractedTextAudioPaths = ExtractTextAudioPaths(dialogueAndOptions);

        yield return new WaitUntil(() => extractedTextAudioPaths.IsCompleted);

        foreach (Dialogues dialogues in extractedTextAudioPaths.Result)
        {
            for (int i = 0; i < dialogues.TextAudioPath.Length; i++)
            {
                string audioName = $"{dialogues.EntityName}-{dialogues.VoiceID}-{i}";

                awsPollyDialogueTriggerEvent.Invoke(new AWSPollyAudioPacket { AudioPath = $"{PersistencePath}\\{audioName}", AudioName = audioName, AudioVoiceId = dialogues.VoiceID, DialogueText = dialogues.TextAudioPath[i].Sentence });

                yield return new WaitUntil(() => AudioGenerated == true);

                dialogues.TextAudioPath[i].AudioPath = $"{PersistencePath}\\{audioName}";

                AudioGenerated = false;
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

    private void AudioGeneratedListener(bool audioGenerated)
    {
        AudioGenerated = audioGenerated;
    }

    public void Preload(DialoguesAndOptions dialogueAndOptions)
    {
        StartCoroutine(PreloadAudio(dialogueAndOptions));
    }

    public override async Task<Tuple<EntityType, dynamic>> EntityPreloadAction(AssetReference assetReference, EntityType entityType, Preloader preloader)
    {
        //find a way to pass the audio and options directly here??? 

        GameObject audioPreloadInstance = (GameObject) await preloader.PreloadAsset<GameObject>(assetReference, entityType);

        return new Tuple<EntityType, dynamic>(EntityType.MonoBehavior , audioPreloadInstance);
    }

    //use an event to use preloader + send it for action

}

