using System.Threading.Tasks;
using UnityEngine.Events;

public class HealthEvent : UnityEventWTAsync<float>
{
    private UnityEvent<float> m_healthEvent = new UnityEvent<float>();
    public override Task AddListener(UnityAction<float> action)
    {
        m_healthEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<float> GetInstance()
    {
        return m_healthEvent;
    }

    public override Task Invoke(float value)
    {
        m_healthEvent.Invoke(value);  

        return Task.CompletedTask;
    }
}