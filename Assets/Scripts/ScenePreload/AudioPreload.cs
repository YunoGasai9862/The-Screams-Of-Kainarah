
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class AudioPreload : EntityPreloadMonoBehavior, IPreloadAudio<DialoguesAndOptions>, IMediatorNotificationListener, IDelegate, IActiveNotifier
{
    private string PersistencePath { get; set; }

    private bool AudioGenerated { get; set; } = false;

    private EntityPoolManager EntityPoolManager { get; set; }

    private DialoguesAndOptions DialoguesAndOptions { get; set; }
    
    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private IMediator Mediator { get; set; }
    
    [SerializeField]
    AWSPollyDialogueTriggerEvent awsPollyDialogueTriggerEvent;
    [SerializeField]
    AudioGeneratedEvent audioGeneratedEvent;

    private void Awake()
    {
        PersistencePath = Application.persistentDataPath;

    }

    private async void Start()
    {
        //Do this during preloadign screen - another class for that already (GameLoad.cs) with loading UIIActiveNotifier
        await audioGeneratedEvent.AddListener(AudioGeneratedListener);

        InvokeCustomMethod += GetDialoguesAndOptions;
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

    public override async Task<Tuple<EntityType, dynamic>> EntityPreload(dynamic assetReference, Asset entityType, Preloader preloader)
    {
        //GameObject audioPreloadInstance = (GameObject) await preloader.PreloadAsset<GameObject>(assetReference, entityType);

        //return new Tuple<EntityType, dynamic>(EntityType.MonoBehavior , audioPreloadInstance);
        return null;
    }

    public async void GetDialoguesAndOptions()
    {
        EntityPoolManager = await GetEntityManager();

        EntityPool<UnityEngine.Object> dialogues = (EntityPool<UnityEngine.Object>) await EntityPoolManager.GetPooledEntity(Constants.DIALOGUES_AND_OPTIONS);

        DialoguesAndOptions = (DialoguesAndOptions) (dialogues.Entity as ScriptableObject);
    }

    public Task<EntityPoolManager> GetEntityManager()
    {
        return Task.FromResult(SceneSingleton.EntityPoolManager);
    }

    public async Task NotifyAboutActivation()
    {
        //dont use this bilal, maybe initiliaze in order, get the mediator out there first and let it scream so the childs can know that its active, and then they can relay anyting back to it.
        //let mediator notify the objecst - it should be initialized at last especially during the preloading period
        await Mediator.NotifyManager(new NotifyPackage { EntityNameToNotify = "NotificationManager", NotifierEntity = new NotifierEntity { IsActive = true, Tag = this.name } });
    }

    public async Task MediatorNotificationListener()
    {
        //lets call this here when mediator pings back
        await NotifyAboutActivation();
    }
}

