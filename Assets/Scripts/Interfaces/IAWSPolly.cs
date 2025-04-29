

using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using UnityEngine;
public interface IAWSPolly
{
    abstract Task<BasicAWSCredentials> SetBasicAWSCredentials(AWSAccessResource awsAccessResource);
    abstract Task<AmazonPollyClient> EstablishConnection(BasicAWSCredentials credentials, RegionEndpoint region);
    abstract Task<SynthesizeSpeechResponse> AWSSynthesizeSpeechCommunicator(AmazonPollyClient client, string text, Engine engine, VoiceId voiceId, OutputFormat outputFormat);
    abstract IEnumerator<WaitUntil> GenerateAudio(AWSPollyAudioPacket awsPollyAudioPacket);
}