using System.Threading.Tasks;
using UnityEngine.Events;

public class CrystalCollideEvent : UnityEventWT<bool>
{
    private UnityEvent<bool> _crystalCollideEvent = new UnityEvent<bool>();
    public override Task AddListener(UnityAction<bool> action)
    {
        _crystalCollideEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override UnityEvent<bool> GetInstance()
    {
        return _crystalCollideEvent;
    }
}