

using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
public interface IAWSPolly
{
    abstract Task<AmazonPollyClient> EstablishConnection(BasicAWSCredentials credentials, RegionEndpoint region);
    abstract Task SetCredentials();

    abstract Task<SynthesizeSpeechResponse> AWSSynthesizeSpeechCommunicator(AmazonPollyClient client, string text, Engine engine, VoiceId voiceId, OutputFormat outputFormat);
 
}