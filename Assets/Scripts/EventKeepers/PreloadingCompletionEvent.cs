

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class PreloadingCompletionEvent : UnityEventWTAsync
{
    private UnityEvent m_preloadingCompletionEvent = new UnityEvent();
    public override UnityEvent GetInstance()
    {
        return m_preloadingCompletionEvent;
    }
    public override Task AddListener(UnityAction action)
    {
        m_preloadingCompletionEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke()
    {
        m_preloadingCompletionEvent.Invoke();

        return Task.CompletedTask;
    }
}