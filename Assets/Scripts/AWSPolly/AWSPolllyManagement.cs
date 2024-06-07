
using UnityEngine;
using Amazon.Polly;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;

public class AWSPolllyManagement : MonoBehaviour, IAWSPolly
{
    private BasicAWSCredentials credentials { get; set; }

    private async void Start()
    {
        await SetCredentials();
        await EstablishConnection(credentials, RegionEndpoint.EUCentral1);
    }

    public async Task EstablishConnection(BasicAWSCredentials credentials, RegionEndpoint endpoint)
    {
        try
        {
            AmazonPollyClient client = new AmazonPollyClient(credentials, endpoint);

        }catch(System.Exception e)
        {
            Debug.Log($"Exception: {e.Message}");
        }

        await Task.Delay(0);
    }

    public Task SetCredentials()
    {
        credentials = new BasicAWSCredentials("", ""); //access and secret key

        return Task.CompletedTask;
    }

    public Task<SynthesizeSpeechRequest> AWSSynthesizeSpeechRequest()
    {
        return null;
    }

    public SynthesizeSpeechRequest SetFields(string text, Engine engine, VoiceId voiceId, OutputFormat outputFormat)
    {
        return new SynthesizeSpeechRequest()
        {
            Text = text,
            Engine = engine,
            VoiceId = voiceId,
            OutputFormat = outputFormat
        };
    }
}
