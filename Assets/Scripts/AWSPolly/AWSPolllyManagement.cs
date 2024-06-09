
using UnityEngine;
using Amazon.Polly;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;
using System;

public class AWSPolllyManagement : MonoBehaviour, IAWSPolly
{
    private BasicAWSCredentials Credentials { get; set; }

    private AmazonPollyClient AmazonPollyClient { get; set; }

    private SynthesizeSpeechResponse SynthesizeSpeechResponse { get; set; }

    private FileUtils FileUtils { get; set; } = new FileUtils();

    private UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; } = new UnityWebRequestMultimediaManager();

    private string AudioPath { get; set; } = "Audio.mp3";


    private async void Start()
    {
        await SetCredentials();

        AmazonPollyClient = await EstablishConnection(Credentials, RegionEndpoint.EUCentral1);

        //use event system to invoke these, but for now check if it works on the start method
        SynthesizeSpeechResponse = await AWSSynthesizeSpeechCommunicator(AmazonPollyClient, "TESTING FOR THE FIRST TIMME", Engine.Neural, VoiceId.Aditi, OutputFormat.Mp3);

        //make this dynamic too, save the audio on a better pattern
        await SaveAudio(SynthesizeSpeechResponse, $"{Application.persistentDataPath}/{AudioPath}");

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
        Credentials = new BasicAWSCredentials("", ""); //access and secret key

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
