
using UnityEngine;
using Amazon.Polly;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;
using System;
using System.Threading;
using System.Collections.Generic;

[AssetAttribute(Asset.MONOBEHAVIOR, "AWSPollyManager", InstantiationOrder = 6)]
public class AWSPolllyManagement : MonoBehaviour, IAWSPolly, IObserver<FirebaseStorageManager>, IObserver<AsyncCoroutine>, ISubject<IAWSPolly>
{
    //gs://the-screams-of-kainarah.appspot.com
    //AWSKeys.txt
    private const int AWS_ACCESS_KEY_INDEX = 0;

    private const int SECRET_AWS_ASCCESS_KEY_INDEX = 1;

    private BasicAWSCredentials Credentials { get; set; }

    private AmazonPollyClient AmazonPollyClient { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private FileUtils FileUtils { get; set; } = new FileUtils();

    private SemaphoreSlim AWSSemaphore { get; set; } = new SemaphoreSlim(1);

    private FirebaseStorageManager FirebaseStorageManagerInstance { get; set; }

    private AWSAccessResource AWSAccessResource { get; set; }

    private AsyncCoroutine AsyncCoroutine { get; set; }

    private FirebaseStorageManagerDelegator FirebaseStorageManagerDelegator { get; set; }

    private AWSPollyManagementDelegator AWSPollyManagementDelegator { get; set; }

    private AsyncCoroutineDelegator AsyncCoroutineDelegator { get; set; }

    [SerializeField]
    string FirebaseStorageURL;
    [SerializeField]
    string AWSKeysfileNameOnFireBase;
    [SerializeField]
    AudioGeneratedEvent audioGeneratedEvent;

    private void Awake()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }

    private async void Start()
    {
        FirebaseStorageManagerDelegator = await Helper.GetDelegator<FirebaseStorageManagerDelegator>();

        AsyncCoroutineDelegator = await Helper.GetDelegator<AsyncCoroutineDelegator>();

        AWSPollyManagementDelegator = await Helper.GetDelegator<AWSPollyManagementDelegator>();

        FirebaseStorageManagerDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(FirebaseStorageManager).ToString()), CancellationToken);

        AsyncCoroutineDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AsyncCoroutine).ToString()), CancellationToken);

        AWSPollyManagementDelegator.AddToSubjectsDict(typeof(AWSPolllyManagement).ToString(), name, new Subject<IAWSPolly>(this, typeof(AWSPolllyManagement)));
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
    
    public async IAsyncEnumerator<WaitUntil> GenerateAudioAsync(AmazonPollyClient amazonPollyClient, AWSPollyAudioPacket awsPollyAudioPacket)
    {
        SynthesizeSpeechResponse synthesizeSpeechResponse = await AWSSynthesizeSpeechCommunicator(AmazonPollyClient, awsPollyAudioPacket.DialogueText, Engine.Standard, awsPollyAudioPacket.AudioVoiceId, awsPollyAudioPacket.OutputFormat).ConfigureAwait(false);

        await SaveAudio(synthesizeSpeechResponse, awsPollyAudioPacket.AudioPath).ConfigureAwait(false);

        await audioGeneratedEvent.Invoke(true);

        //Wait Until won't work for async coroutines - that's for standard coroutines in unity
        yield return null;
    }

    public IEnumerator<WaitUntil> OffloadExecutionToAsyncRunner(AWSPollyAudioPacket awsPollyAudioPacket)
    {
        yield return new WaitUntil(() => !Helper.IsObjectNull(AsyncCoroutine) && !Helper.IsObjectNull(AmazonPollyClient));

        AsyncCoroutine.ExecuteAsyncCoroutine(GenerateAudioAsync(AmazonPollyClient, awsPollyAudioPacket));

    }

    public async Task GenerateAudio(AWSPollyAudioPacket aWSPollyAudioPacket)
    {
        StartCoroutine(OffloadExecutionToAsyncRunner(aWSPollyAudioPacket));
    }

    private Task SaveAudio(SynthesizeSpeechResponse response, string fullPath)
    {
        FileUtils.WriteToFile(response.AudioStream, fullPath);

        return Task.CompletedTask;
    }

    public void OnNotifySubject(IObserver<IAWSPolly> data, Context context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(AWSPollyManagementDelegator.NotifyObserver(data, this, context, cancellationToken, semaphoreSlim));
    }

    public async void OnNotify(FirebaseStorageManager data, Context context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        FirebaseStorageManagerInstance = data;

        AWSAccessResource = await RetrieveAWSKeys();

        Credentials = await SetBasicAWSCredentials(AWSAccessResource);

        AmazonPollyClient = await EstablishConnection(Credentials, RegionEndpoint.EUCentral1);
    }

    public void OnNotify(AsyncCoroutine data, Context context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    
}
