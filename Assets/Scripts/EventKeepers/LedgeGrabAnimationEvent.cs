using System.Threading.Tasks;
using UnityEngine.Events;
public class LedgeGrabAnimationEvent : UnityEventWT<bool>
{
    private UnityEvent<bool> _instance = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        return _instance;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        _instance.AddListener(action);
       
        return Task.CompletedTask;
    }
}