using System.Threading.Tasks;
using UnityEngine.Events;


//ADD A BASE EVENT in the parameter so can be used by subclasses :_)))
public class StateEvent<T> : UnityEventWTAsync<T>
{
    private UnityEvent<T> m_stateEvent = new UnityEvent<T>();
    public override Task AddListener(UnityAction<T> action)
    {
        m_stateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override Task AddListener(UnityAction<T> action)
    {
        throw new System.NotImplementedException();
    }

    public override UnityEvent<PlayerState> GetInstance()
    {
        return m_gameStateEvent;
    }

    public override Task Invoke(T value)
    {
        m_stateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}