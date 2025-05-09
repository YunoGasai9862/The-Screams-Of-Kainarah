using Amazon.Polly;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

public class AudioTriggerEvent : UnityEventWTAsync<AudioPackage>
{
    private UnityEvent<AudioPackage> m_AudioTriggerEvent = new UnityEvent<AudioPackage>();    
    public override Task AddListener(UnityAction<AudioPackage> audioPackage)
    {
        m_AudioTriggerEvent.AddListener(audioPackage);

        return Task.CompletedTask;
    }

    public override UnityEvent<AudioPackage> GetInstance()
    {
        return m_AudioTriggerEvent;
    }

    public override Task Invoke(AudioPackage audioPackage)
    {
        m_AudioTriggerEvent.Invoke(audioPackage);

        return Task.CompletedTask;
    }
}