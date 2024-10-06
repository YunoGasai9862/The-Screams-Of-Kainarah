using System.Threading.Tasks;
using UnityEngine.Events;
public class ExecutePreloadingEvent : UnityEventWTAsync
{
    private UnityEvent m_executingPreloadingEvent = new UnityEvent();
    public override UnityEvent GetInstance()
    {
        return m_executingPreloadingEvent;
    }
    public override Task AddListener(UnityAction action)
    {
        m_executingPreloadingEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke()
    {
        m_executingPreloadingEvent.Invoke();

        return Task.CompletedTask;
    }
}