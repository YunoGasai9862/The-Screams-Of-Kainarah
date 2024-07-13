using System.Threading.Tasks;
using UnityEngine.Events;

public class NextDialogueTriggerEvent : UnityEventWT<bool>
{
    private UnityEvent<bool> m_nextDialogueTriggerEvent = new UnityEvent<bool>();
    public override void AddListener(UnityAction<bool> action)
    {
        m_nextDialogueTriggerEvent.AddListener(action);
    }

    public override UnityEvent<bool> GetInstance()
    {
        return m_nextDialogueTriggerEvent;
    }

    public override void Invoke(bool value)
    {
        m_nextDialogueTriggerEvent.Invoke(value);
    }
}