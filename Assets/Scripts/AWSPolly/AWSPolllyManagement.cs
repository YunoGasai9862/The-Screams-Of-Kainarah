
using UnityEngine;
using Amazon.Polly;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;
using System;
using System.IO;
using System.Threading;

public class AWSPolllyManagement : MonoBehaviour, IAWSPolly
{
    private const int AWS_ACCESS_KEY_INDEX = 0;

    private const int SECRET_AWS_ASCCESS_KEY_INDEX = 1;

    private string AWS_ACCESS_KEY { get; set; }

    private string SECRET_AWS_ASCCESS_KEY { get; set; }

    private BasicAWSCredentials Credentials { get; set; }

    private AmazonPollyClient AmazonPollyClient { get; set; }

    private SynthesizeSpeechResponse SynthesizeSpeechResponse { get; set; }

    private FileUtils FileUtils { get; set; } = new FileUtils();

    private UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; } = new UnityWebRequestMultimediaManager();

    private int VOICE_GENERATION_DELAY { get; set; } = 500;

    private SemaphoreSlim AWSSemaphore { get; set; } = new SemaphoreSlim(1);
    private string AudioPath { get; set; } = "AmazonNarrator.mp3";
    private string PersistencePath { get; set; }


    [SerializeField]
    AudioSource AudioSource;
    [SerializeField]
    FirebaseStorageManager FirebaseStorageManager;
    [SerializeField]
    string FirebaseStorageURL;
    [SerializeField]
    string AWSKeysfileNameOnFireBase;
    [SerializeField]
    AWSPollyDialogueTriggerEvent m_AWSPollyDialogueTriggerEvent;

    private void Awake()
    {
        PersistencePath = $"{Application.persistentDataPath}/{AudioPath}";
    }

    private async void Start()
    {
        await m_AWSPollyDialogueTriggerEvent.AddListener(GenerateAIVoice);

        await SetupFirebaseStorageForAWSPrivateKeys();
        
        await SetCredentials();

        AmazonPollyClient = await EstablishConnection(Credentials, RegionEndpoint.EUCentral1);

    }


    public async Task SetupFirebaseStorageForAWSPrivateKeys()
    {
        FirebaseStorageManager.SetFirebaseStorageLocation(FirebaseStorageURL);

        TextAsset keys = await FirebaseStorageManager.DownloadMedia<TextAsset>(FileType.TEXT, AWSKeysfileNameOnFireBase);

        string[] splitKeys = await Helper.SplitStringOnSeparator(keys.text, "|");

        AWS_ACCESS_KEY = splitKeys[AWS_ACCESS_KEY_INDEX];

        SECRET_AWS_ASCCESS_KEY = splitKeys[SECRET_AWS_ASCCESS_KEY_INDEX];
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

    public Task SetCredentials()
    {
        //use DB to fetch it
        Credentials = new BasicAWSCredentials(AWS_ACCESS_KEY, SECRET_AWS_ASCCESS_KEY); //access and secret key to be fetched from firebase

        return Task.CompletedTask;
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
            //look for more enhancements + better solutioning
           // await Task.Delay(VOICE_GENERATION_DELAY).ConfigureAwait(false);

           // return response;

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
    
    //this will be invoked by an event
    public async void GenerateAIVoice(string text, VoiceId voiceId)
    {
        SynthesizeSpeechResponse = await AWSSynthesizeSpeechCommunicator(AmazonPollyClient, text, Engine.Neural, voiceId, OutputFormat.Mp3).ConfigureAwait(false);

        Debug.Log(SynthesizeSpeechResponse);

        await SaveAudio(SynthesizeSpeechResponse, PersistencePath).ConfigureAwait(false);

        AudioSource.clip = await UnityWebRequestMultimediaManager.GetAudio(PersistencePath, AudioPath, AudioType.MPEG);

        AudioSource.Play();
   
    }

    private Task SaveAudio(SynthesizeSpeechResponse response, string fullPath)
    {
        FileUtils.WriteToFile(response.AudioStream, fullPath);

        return Task.CompletedTask;
    }

}
