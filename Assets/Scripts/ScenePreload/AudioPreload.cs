
using Amazon.Polly;
using Assets.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.Scene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(AudioPreload), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(AudioPreload), SubjectType = typeof(AWSPolllyManagement), ContextType = typeof(IAWSPolly))]
[Asset(Asset.MONOBEHAVIOR, "Audio", InstantiationOrder = 8)]
public class AudioPreload : MonoBehaviorScene, IPreloadAudio<DialoguesAndOptions>, IDelegate, INotify<EntityPoolManager>, INotify<IAWSPolly>
{
    private string PersistencePath { get; set; }

    private bool AudioGenerated { get; set; } = false;

    private DialoguesAndOptions DialoguesAndOptions { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    public IAWSPolly AWSPollyManager { get; set; }

    private Delegator Delegator { get; set; }

    private AudioGeneratedEvent m_audioGeneratedEvent;

    private SceneUtils SceneUtils { get; set; }


    private void Awake()
    {
        PersistencePath = Application.persistentDataPath;

        InvokeCustomMethod += Preload;
    }
    private async void Start()
    {
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        m_audioGeneratedEvent = await SceneUtils.GetCustomEvent<AudioGeneratedEvent>();

        await m_audioGeneratedEvent.AddListener(AudioGeneratedListener);

        Delegator.NotifySubjectWrapper(SceneUtils.BuildNotificationContext<EntityPoolManager>(gameObject, typeof(EntityPoolManager), typeof(AudioPreload)), this);

        Delegator.NotifySubjectWrapper(SceneUtils.BuildNotificationContext<IAWSPolly>(gameObject, typeof(AWSPolllyManagement), typeof(AudioPreload)), this);
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

                if (SceneUtils.DoesFileExist(audioPath))
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

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManager = value;

        yield return null;
    }

    public IEnumerator Notify(IAWSPolly value)
    {
        AWSPollyManager = value;

        yield return null;
    }

    public override void Broadcast(dynamic value)
    {
        if (value is SceneUtils && SceneUtils == null)
        {
            SceneUtils = value as SceneUtils;
        }
    }
}

