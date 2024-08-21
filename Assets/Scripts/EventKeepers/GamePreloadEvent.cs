using UnityEngine.Events;
using System.Threading.Tasks;
public class GamePreloadEvent<T, TAction>: UnityEventWTAsync<T, TAction>
{
    UnityEvent<T, TAction> m_preloadEvent = new UnityEvent<T, TAction>();

    public override UnityEvent<T, TAction> GetInstance()
    {
        return m_preloadEvent;
    }
    public override Task AddListener(UnityAction<T, TAction> action) 
    {
        m_preloadEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(T tValue, TAction zValue)
    {
        m_preloadEvent.Invoke(tValue, zValue);

        return Task.CompletedTask;
    }
} 