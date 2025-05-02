using System.Threading.Tasks;
using UnityEngine.Events;

public class AudioGeneratedEvent : UnityEventWTAsync<bool>
{
    private UnityEvent<bool> m_AudioGenerated = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        return m_AudioGenerated;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        m_AudioGenerated.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(bool value)
    {
        m_AudioGenerated.Invoke(value);

        return Task.CompletedTask;
    }
}