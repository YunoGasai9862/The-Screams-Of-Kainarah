using System.Threading.Tasks;
using UnityEngine.Events;

public class PowerUpBarFillEvent: UnityEventWT<bool>
{
    private UnityEvent<bool> _powerUpBarFillEvent = new UnityEvent<bool>();
    public override Task AddListener(UnityAction<bool> action)
    {
        _powerUpBarFillEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<bool> GetInstance()
    {
        return _powerUpBarFillEvent;
    }
}