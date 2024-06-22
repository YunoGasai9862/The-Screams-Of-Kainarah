using System.Threading.Tasks;
using UnityEngine.Events;

public class DialogueTakingPlaceEvent : UnityEventWT<bool>
{
    private UnityEvent<bool> m_DialogueTakingPlaceEvent = new UnityEvent<bool>();
    public override Task AddListener(UnityAction<bool> action)
    {
        m_DialogueTakingPlaceEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<bool> GetInstance()
    {
        return m_DialogueTakingPlaceEvent;
    }

    public override Task Invoke(bool value)
    {
        m_DialogueTakingPlaceEvent.Invoke(value);

        return Task.CompletedTask;
    }
}