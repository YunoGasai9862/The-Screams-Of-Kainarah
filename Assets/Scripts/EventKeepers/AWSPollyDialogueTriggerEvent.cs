using Amazon.Polly;
using System.Threading.Tasks;
using UnityEngine.Events;

public class AWSPollyDialogueTriggerEvent : UnityEventWT<string, VoiceId>
{
    private UnityEvent<string, VoiceId> m_amazonPollyDialogueTriggerEvent = new UnityEvent<string, VoiceId>();    
    public override Task AddListener(UnityAction<string, VoiceId> action)
    {
        m_amazonPollyDialogueTriggerEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<string, VoiceId> GetInstance()
    {
        return m_amazonPollyDialogueTriggerEvent;
    }

    public override Task Invoke(string value, VoiceId voiceId)
    {
        m_amazonPollyDialogueTriggerEvent.Invoke(value, voiceId);

        return Task.CompletedTask;
    }
}