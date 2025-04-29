
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Asset(AssetType = Asset.MONOBEHAVIOR, AddressLabel = "Audio")]
public class AudioPreload : MonoBehaviour, IPreloadAudio<DialoguesAndOptions>, IDelegate, IObserver<EntityPoolManager>, IObserver<IAWSPolly>
{
    private string PersistencePath { get; set; }

    private bool AudioGenerated { get; set; } = false;

    private DialoguesAndOptions DialoguesAndOptions { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    public IAWSPolly AWSPollyManager { get; set; }

    private EntityPoolManagerDelegator m_entityPoolManagerDelegator;

    private AWSPollyManagementDelegator m_awsPollyManagementDelegator;

    [SerializeField]
    AudioGeneratedEvent audioGeneratedEvent;
   

    private void Awake()
    {
        PersistencePath = Application.persistentDataPath;

        InvokeCustomMethod += Preload;
    }
    private async void Start()
    {
        m_entityPoolManagerDelegator = Helper.GetDelegator<EntityPoolManagerDelegator>();

        m_awsPollyManagementDelegator = Helper.GetDelegator<AWSPollyManagementDelegator>();

        StartCoroutine(m_entityPoolManagerDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EntityPoolManager).ToString()), CancellationToken.None));

        StartCoroutine(m_awsPollyManagementDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(IAWSPolly).ToString()), CancellationToken.None));

        await audioGeneratedEvent.AddListener(AudioGeneratedListener);
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

                Debug.Log($"{audioName}");

                StartCoroutine(AWSPollyManager.GenerateAudio(new AWSPollyAudioPacket { AudioPath = $"{PersistencePath}\\{audioName}", AudioName = audioName, AudioVoiceId = dialogues.VoiceID, DialogueText = dialogues.TextAudioPath[i].Sentence }));

                yield return new WaitUntil(() => AudioGenerated == true);

                dialogues.TextAudioPath[i].AudioPath = $"{PersistencePath}\\{audioName}";

                Debug.Log(dialogues.TextAudioPath[i].AudioPath);

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

    public void Preload()
    {
        StartCoroutine(FetchDialoguesAndOptionsAndPreloadAudio());
    }

    private IEnumerator FetchDialoguesAndOptionsAndPreloadAudio()
    {
        yield return new WaitUntil(() => EntityPoolManager != null && AWSPollyManager != null);

        EntityPool dialogues = EntityPoolManager.GetPooledEntity(Constants.DIALOGUES_AND_OPTIONS);

        DialoguesAndOptions = (DialoguesAndOptions)(dialogues.Entity);

        StartCoroutine(PreloadAudio(DialoguesAndOptions));
    }

    public void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EntityPoolManager = data;
    }

    public void OnNotify(IAWSPolly data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AWSPollyManager = data;
    }
}

