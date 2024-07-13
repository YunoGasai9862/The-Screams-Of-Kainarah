using System.Threading.Tasks;
using UnityEngine.Events;
public class LedgeGrabAnimationEvent : UnityEventWTAsync<bool>
{
    private UnityEvent<bool> m_ledgeGrabAnimationEvent = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        return m_ledgeGrabAnimationEvent;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        m_ledgeGrabAnimationEvent.AddListener(action);
       
        return Task.CompletedTask;
    }

    public override Task Invoke(bool value)
    {
        m_ledgeGrabAnimationEvent.Invoke(value);

        return Task.CompletedTask;
    }
}