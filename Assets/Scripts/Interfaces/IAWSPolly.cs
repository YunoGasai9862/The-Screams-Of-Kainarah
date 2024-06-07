

using System.Threading.Tasks;
using Amazon;
using Amazon.Polly.Model;
using Amazon.Runtime;
public interface IAWSPolly
{
    abstract Task EstablishConnection(BasicAWSCredentials credentials, RegionEndpoint region);
    abstract Task SetCredentials();

    abstract Task<SynthesizeSpeechRequest> AWSSynthesizeSpeechRequest();
 
}