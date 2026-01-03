
using Amazon.Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Asset(Asset.MONOBEHAVIOR, "Audio", InstantiationOrder = 6)]
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

    private AudioGeneratedEvent m_audioGeneratedEvent;
   

    private void Awake()
    {
        PersistencePath = Application.persistentDataPath;

        InvokeCustomMethod += Preload;
    }
    private async void Start()
    {
        m_entityPoolManagerDelegator = await Helper.GetDelegator<EntityPoolManagerDelegator>();

        m_awsPollyManagementDelegator = await Helper.GetDelegator<AWSPollyManagementDelegator>();

        m_audioGeneratedEvent = await Helper.GetCustomEvent<AudioGeneratedEvent>();

        await m_audioGeneratedEvent.AddListener(AudioGeneratedListener);

        m_entityPoolManagerDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EntityPoolManager).ToString()), CancellationToken.None);

        m_awsPollyManagementDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AWSPolllyManagement).ToString()), CancellationToken.None);
    }

    public IEnumerator PreloadAudio(DialoguesAndOptions dialogueAndOptions)
    {
        Task<List<DialogueSetup>> extractedTextAudioPaths = ExtractTextAudioPaths(dialogueAndOptions);

        yield return new WaitUntil(() => extractedTextAudioPaths.IsCompleted);

        foreach (DialogueSetup dialogues in extractedTextAudioPaths.Result)
        {
            for (int i = 0; i < dialogues.Dialogues.Length; i++)
            {                
                string audioName = $"{dialogues.EntityName}-{dialogues.VoiceID}-{i}";

                string audioPath = $"{PersistencePath}\\{audioName}.{OutputFormat.FindValue(OutputFormat.Mp3)}";

                dialogues.Dialogues[i].AudioInfo.AudioPath = audioPath;

                if (Helper.DoesFileExist(audioPath))
                {
                    continue;
                }

                AWSPollyManager.GenerateAudio(new AWSPollyAudioPacket { AudioPath = audioPath, AudioName = audioName, AudioVoiceId = dialogues.VoiceID, DialogueText = dialogues.Dialogues[i].Sentence, OutputFormat = OutputFormat.Mp3});

                yield return new WaitUntil(() => AudioGenerated == true);


                AudioGenerated = false;
            }
        }
    }


    private Task<List<DialogueSetup>> ExtractTextAudioPaths(DialoguesAndOptions dialoguesAndOptions)
    {
        List<AudioInfo> textAudioPath = new List<AudioInfo>();
        TaskCompletionSource<List<DialogueSetup>> tcs = new TaskCompletionSource<List<DialogueSetup>>();   
        Task.Run(() =>
        {
            tcs.SetResult(dialoguesAndOptions.exchange.
               SelectMany(dialogues => dialogues.DialogueSetup).ToList());

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

        List<EntityPool> dialogues = EntityPoolManager.GetPooledEntity(Constants.DIALOGUES_AND_OPTIONS);

        dialogues.ForEach(dialogue =>
        {
            DialoguesAndOptions = (DialoguesAndOptions)(dialogue.Entity);

            StartCoroutine(PreloadAudio(DialoguesAndOptions));
        });
    }

    public void OnNotify(EntityPoolManager data, Context context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EntityPoolManager = data;
    }

    public void OnNotify(IAWSPolly data, Context context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AWSPollyManager = data;
    }
}

