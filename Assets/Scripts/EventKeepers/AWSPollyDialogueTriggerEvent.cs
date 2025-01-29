using Amazon.Polly;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

public class AWSPollyDialogueTriggerEvent : UnityEventWTAsync<AWSPollyAudioPacket>
{
    private UnityEvent<AWSPollyAudioPacket> m_amazonPollyDialogueTriggerEvent = new UnityEvent<AWSPollyAudioPacket>();    
    public override Task AddListener(UnityAction<AWSPollyAudioPacket> awsPollyAudioPacket)
    {
        m_amazonPollyDialogueTriggerEvent.AddListener(awsPollyAudioPacket);

        return Task.CompletedTask;
    }

    public override UnityEvent<AWSPollyAudioPacket> GetInstance()
    {
        return m_amazonPollyDialogueTriggerEvent;
    }

    public override Task Invoke(AWSPollyAudioPacket awsPollyAudioPacket )
    {
        Debug.Log(awsPollyAudioPacket.ToString());

        m_amazonPollyDialogueTriggerEvent.Invoke(awsPollyAudioPacket);

        return Task.CompletedTask;
    }
}