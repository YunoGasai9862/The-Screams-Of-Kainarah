

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class PreloadingCompletionEvent : UnityEventWTAsync<bool>
{
    private UnityEvent<bool> m_preloadingCompletionEvent = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        return m_preloadingCompletionEvent;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        m_preloadingCompletionEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(bool value)
    {
        m_preloadingCompletionEvent.Invoke(value);

        return Task.CompletedTask;
    }
}