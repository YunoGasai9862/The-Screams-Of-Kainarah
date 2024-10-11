
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

    private EntityPoolManager EntityPoolManager { get; set; }

    private EntityPool<ScriptableObject> DialoguesAndOptions { get; set; }

    [SerializeField]
    AWSPollyDialogueTriggerEvent awsPollyDialogueTriggerEvent;
    [SerializeField]
    DialoguesAndOptions dialogueAndOptions;
    [SerializeField]
    AudioGeneratedEvent audioGeneratedEvent;
    [SerializeField]
    PreloadingCompletionEvent preloadingCompletionEvent;

    private void Awake()
    {
        PersistencePath = Application.persistentDataPath;

        //OMG this worked!!
        preloadingCompletionEvent.AddListener(PreloadingCompletionEventListener);
    }


    private void Start()
    {
        //Do this during preloadign screen - another class for that already (GameLoad.cs) with loading UI
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

    public override async Task<Tuple<EntityType, dynamic>> EntityPreload(AssetReference assetReference, EntityType entityType, Preloader preloader)
    {
        GameObject audioPreloadInstance = (GameObject) await preloader.PreloadAsset<GameObject>(assetReference, entityType);

        await preloadingCompletionEvent.Invoke();

        return new Tuple<EntityType, dynamic>(EntityType.MonoBehavior , audioPreloadInstance);
    }

    public async void PreloadingCompletionEventListener()
    {
        Debug.Log("Here");

        EntityPoolManager = await GetEntityManager();

        DialoguesAndOptions = await EntityPoolManager.GetPooledEntity(Constants.DIALOGUES_AND_OPTIONS) as EntityPool<ScriptableObject>;

        Debug.Log($"{DialoguesAndOptions.ToString()}");
    }

    public Task<EntityPoolManager> GetEntityManager()
    {
        return Task.FromResult(SceneSingleton.EntityPoolManager);
    }
}

