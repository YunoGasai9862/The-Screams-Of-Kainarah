using System.Threading.Tasks;
using UnityEngine.Events;

public class CelestialBodyEvent : UnityEventWT<LightEntity>
{
    private UnityEvent<LightEntity> _celestialBodyEvent = new UnityEvent<LightEntity>();
    public override Task AddListener(UnityAction<LightEntity> action)
    {
        _celestialBodyEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override UnityEvent<LightEntity> GetInstance()
    {
        return _celestialBodyEvent;
    }
}