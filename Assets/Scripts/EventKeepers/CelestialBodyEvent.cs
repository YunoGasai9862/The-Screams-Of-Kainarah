using System.Threading.Tasks;
using UnityEngine.Events;

public class CelestialBodyEvent : UnityEventWTAsync<LightEntity>
{
    private UnityEvent<LightEntity> m_celestialBodyEvent = new UnityEvent<LightEntity>();
    public override Task AddListener(UnityAction<LightEntity> action)
    {
        m_celestialBodyEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override UnityEvent<LightEntity> GetInstance()
    {
        return m_celestialBodyEvent;
    }

    public override Task Invoke(LightEntity value)
    {
        m_celestialBodyEvent.Invoke(value);

        return Task.CompletedTask;
    }
}