using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class UIParticleSystemEvent: UnityEventWT<float>
{
    private UnityEvent<float> _particleSystemEvent = new UnityEvent<float>();
    public override Task AddListener(UnityAction<float> action)
    {
        try
        {
            _particleSystemEvent.AddListener(action);

        }catch(Exception ex)
        {
            throw ex;
        }
        return Task.CompletedTask;
    }

    public override UnityEvent<float> GetInstance()
    {
        return _particleSystemEvent;
    }
}