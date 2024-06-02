using System.Threading.Tasks;
using UnityEngine.Events;

public class PowerUpBarFillEvent: UnityEventWT<bool>
{
    private UnityEvent<bool> m_powerUpBarFillEvent = new UnityEvent<bool>();

    public override Task AddListener(UnityAction<bool> action)
    {
        m_powerUpBarFillEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override UnityEvent<bool> GetInstance()
    {
        return m_powerUpBarFillEvent;
    }

    public override Task Invoke(bool value)
    {
        m_powerUpBarFillEvent.Invoke(value);

        return Task.CompletedTask;
    }
}