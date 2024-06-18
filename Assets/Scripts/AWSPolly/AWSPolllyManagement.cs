
using UnityEngine;
using Amazon.Polly;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;
using System;

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

    //use memory space for writing
    private string AudioPath { get; set; } = "AmazonNarrator.mp3";

    [SerializeField]
    AudioSource AudioSource;
    [SerializeField]
    FirebaseStorageManager FirebaseStorageManager;
    [SerializeField]
    string FirebaseStorageURL;
    [SerializeField]
    string AWSKeysfileNameOnFireBase;

    private async void Start()
    {
        await SetupFirebaseStorageForAWSPrivateKeys();
        
        await SetCredentials();

        AmazonPollyClient = await EstablishConnection(Credentials, RegionEndpoint.EUCentral1);

        //use event system to invoke these, but for now check if it works on the start method
        SynthesizeSpeechResponse = await AWSSynthesizeSpeechCommunicator(AmazonPollyClient, "TESTING FOR THE FIRST TIMME", Engine.Neural, VoiceId.Bianca, OutputFormat.Mp3);

        //make this dynamic too, save the audio on a better pattern
        await SaveAudio(SynthesizeSpeechResponse, $"{Application.persistentDataPath}/{AudioPath}");

        AudioSource.clip = await UnityWebRequestMultimediaManager.GetAudio($"{Application.persistentDataPath}/{AudioPath}", AudioType.MPEG);

        //for testing lets play it
        //AudioSource.Play();

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
        try
        {
            SynthesizeSpeechRequest request = PrepareSynthesizeSpeechRequestPacket(text, engine, voiceId, outputFormat);

            SynthesizeSpeechResponse response = await PrepareSynthesizeSpeechResponsePacket(client, request);

            return response;

        }catch(Exception ex)
        {
            Debug.Log($"Exception: {ex.Message}");
            throw ex;
        }
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


    public Task<SynthesizeSpeechResponse> PrepareSynthesizeSpeechResponsePacket(AmazonPollyClient client, SynthesizeSpeechRequest request)
    {
        return client.SynthesizeSpeechAsync(request);
    }

    private Task SaveAudio(SynthesizeSpeechResponse response, string fullPath)
    {
        FileUtils.WriteToFile(response.AudioStream, fullPath);

        return Task.CompletedTask;
    }
}
