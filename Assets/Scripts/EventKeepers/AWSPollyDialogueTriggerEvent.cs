using System.Threading.Tasks;
using UnityEngine.Events;

public class AWSPollyDialogueTriggerEvent : UnityEventWT<string>
{
    private UnityEvent<string> m_amazonPollyDialogueTriggerEvent = new UnityEvent<string>();    
    public override Task AddListener(UnityAction<string> action)
    {
        m_amazonPollyDialogueTriggerEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<string> GetInstance()
    {
        return m_amazonPollyDialogueTriggerEvent;
    }

    public override Task Invoke(string value)
    {
        m_amazonPollyDialogueTriggerEvent.Invoke(value);

        return Task.CompletedTask;
    }
}