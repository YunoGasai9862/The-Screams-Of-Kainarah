using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

public class CrystalUIIncrementEvent : UnityEventWT<int>
{
    private UnityEvent<int> m_crystalUIIncrementEvent = new UnityEvent<int>();
    public override Task AddListener(UnityAction<int> action)
    {
        m_crystalUIIncrementEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<int> GetInstance()
    {
        return m_crystalUIIncrementEvent;
    }

    public override Task Invoke(int value)
    {
        Debug.Log($"INVOKING: {value}");
        m_crystalUIIncrementEvent.Invoke(value);

        return Task.CompletedTask;
    }
}