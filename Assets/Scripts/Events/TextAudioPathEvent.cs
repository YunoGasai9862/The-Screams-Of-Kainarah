using System.Threading.Tasks;
using UnityEngine.Events;

public class TextAudioPathEvent: UnityEventWTAsync<string>
{
    private UnityEvent<string> m_textAudioPathEvent = new UnityEvent<string>();
    public override UnityEvent<string> GetInstance()
    {
        return m_textAudioPathEvent;
    }
    public override Task AddListener(UnityAction<string> action)
    {
        m_textAudioPathEvent.AddListener(action); 
        
        return Task.CompletedTask;
    }
    public override Task Invoke(string value)
    {
        m_textAudioPathEvent.Invoke(value);

        return Task.CompletedTask;
    }
}