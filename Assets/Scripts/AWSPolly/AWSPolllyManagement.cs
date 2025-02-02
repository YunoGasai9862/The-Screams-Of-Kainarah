
using UnityEngine;
using Amazon.Polly;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;
using System;
using System.Threading;

public class AWSPolllyManagement : MonoBehaviour, IAWSPolly, IObserver<FirebaseStorageManager>
{
    private const int AWS_ACCESS_KEY_INDEX = 0;

    private const int SECRET_AWS_ASCCESS_KEY_INDEX = 1;

    private BasicAWSCredentials Credentials { get; set; }

    private AmazonPollyClient AmazonPollyClient { get; set; }

    private SynthesizeSpeechResponse SynthesizeSpeechResponse { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private FileUtils FileUtils { get; set; } = new FileUtils();

    private UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; } = new UnityWebRequestMultimediaManager();

    private int VOICE_GENERATION_DELAY { get; set; } = 500;

    private SemaphoreSlim AWSSemaphore { get; set; } = new SemaphoreSlim(1);

    private FirebaseStorageManager FirebaseStorageManagerInstance { get; set; }

    private AWSAccessResource AWSAccessResource { get; set; }
    

    [SerializeField]
    AudioSource AudioSource;
    [SerializeField]
    string FirebaseStorageURL;
    [SerializeField]
    string AWSKeysfileNameOnFireBase;
    [SerializeField]
    AWSPollyDialogueTriggerEvent m_AWSPollyDialogueTriggerEvent;
    [SerializeField]
    MainThreadDispatcherEvent mainThreadDispatcherEvent;
    [SerializeField]
    AudioGeneratedEvent audioGeneratedEvent;
    [SerializeField]
    FirebaseStorageManagerDelegator firebaseStorageManagerDelegator;

    private void Awake()
    {
        
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }

    private async void Start()
    {
        await m_AWSPollyDialogueTriggerEvent.AddListener(ProcessAndSaveAINotes);

        StartCoroutine(firebaseStorageManagerDelegator.NotifySubject(this));
    }

    public async Task<AWSAccessResource> RetrieveAWSKeys()
    {
        FirebaseStorageManagerInstance.SetFirebaseStorageLocation(FirebaseStorageURL);

        TextAsset keys = await FirebaseStorageManagerInstance.DownloadMedia<TextAsset>(FileType.TEXT, AWSKeysfileNameOnFireBase);

        string[] splitKeys = await Helper.SplitStringOnSeparator(keys.text, "|");

        return new AWSAccessResource(splitKeys[AWS_ACCESS_KEY_INDEX], splitKeys[SECRET_AWS_ASCCESS_KEY_INDEX]);
    }

    public Task<AmazonPollyClient> EstablishConnection(BasicAWSCredentials credentials, RegionEndpoint endpoint)
    {
        try
        {
            AmazonPollyClient client = new AmazonPollyClient(credentials, endpoint);

            return Task.FromResult(client);

        }catch(System.Exception e)
        {
            Debug.Log($"Exception: {e.Message}");
            throw e;
        }
    }

    public Task<BasicAWSCredentials> SetBasicAWSCredentials(AWSAccessResource awsAccessResource)
    {
        return Task.FromResult(new BasicAWSCredentials(awsAccessResource.AccessKey, awsAccessResource.SecretAccessKey)); 
    }

    public async Task<SynthesizeSpeechResponse> AWSSynthesizeSpeechCommunicator(AmazonPollyClient client, string text, Engine engine, VoiceId voiceId, OutputFormat outputFormat)
    {
        await AWSSemaphore.WaitAsync();

        try
        {
            SynthesizeSpeechRequest request = PrepareSynthesizeSpeechRequestPacket(text, engine, voiceId, outputFormat);

            SynthesizeSpeechResponse response = await PrepareSynthesizeSpeechResponsePacket(client, request);

            if(response!=null && response.AudioStream != null)
            {
                return response;
            }

        }
        catch (Exception ex)
        {
            Debug.Log($"Exception: {ex.Message}");
            throw;
        }
        finally
        {
            AWSSemaphore.Release();
        }

        return null;
    }

    public SynthesizeSpeechRequest PrepareSynthesizeSpeechRequestPacket(string text, Engine engine, VoiceId voiceId, OutputFormat outputFormat)
    {
        return new SynthesizeSpeechRequest()
        {
            Text = text,
            Engine = engine,
            VoiceId = voiceId,
            OutputFormat = outputFormat
        };
    }


    public async Task<SynthesizeSpeechResponse> PrepareSynthesizeSpeechResponsePacket(AmazonPollyClient client, SynthesizeSpeechRequest request)
    {
        return await client.SynthesizeSpeechAsync(request).ConfigureAwait(false);
    }
    
    public async void ProcessAndSaveAINotes(AWSPollyAudioPacket awsPollyAudioPacket)
    {
        Debug.Log("Here Inside Process and Save AI Notes!");

        SynthesizeSpeechResponse = await AWSSynthesizeSpeechCommunicator(AmazonPollyClient, awsPollyAudioPacket.DialogueText, Engine.Neural, awsPollyAudioPacket.AudioVoiceId, OutputFormat.Mp3).ConfigureAwait(false);

        await SaveAudio(SynthesizeSpeechResponse, awsPollyAudioPacket.AudioPath).ConfigureAwait(false);

        await audioGeneratedEvent.Invoke(true);

    }

    //update this method too - invoke to send in audio Path as well
    public async Task InvokeAIVoice(AWSPollyAudioPacket awsPollyAudioPacket) 
    {
        CustomActions customActions = new CustomActions
        {
            Action = action => PlayAudio((AWSPollyAudioPacket)action),
            Parameter = awsPollyAudioPacket

        };

        await mainThreadDispatcherEvent.Invoke(customActions);
    }


    private async void PlayAudio(AWSPollyAudioPacket awsPollyAudioPacket)
    {
        AudioSource.clip = await UnityWebRequestMultimediaManager.GetAudio(awsPollyAudioPacket.AudioPath, awsPollyAudioPacket.AudioName, AudioType.MPEG);

        AudioSource.Play();
    }

    private Task SaveAudio(SynthesizeSpeechResponse response, string fullPath)
    {
        FileUtils.WriteToFile(response.AudioStream, fullPath);

        return Task.CompletedTask;
    }

    public async void OnNotify(FirebaseStorageManager data, params object[] optional)
    {
        FirebaseStorageManagerInstance = data;

        AWSAccessResource = await RetrieveAWSKeys();

        Credentials = await SetBasicAWSCredentials(AWSAccessResource);

        AmazonPollyClient = await EstablishConnection(Credentials, RegionEndpoint.EUCentral1);
    }
}
