using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class UIParticleSystemEvent: UnityEventWTAsync<float>
{
    private UnityEvent<float> m_particleSystemEvent = new UnityEvent<float>();
    public override Task AddListener(UnityAction<float> action)
    {
        try
        {
            m_particleSystemEvent.AddListener(action);

        }catch(Exception ex)
        {
            throw ex;
        }
        return Task.CompletedTask;
    }

    public override UnityEvent<float> GetInstance()
    {
        return m_particleSystemEvent;
    }

    public override Task Invoke(float value)
    {
        m_particleSystemEvent.Invoke(value);

        return Task.CompletedTask;
    }
}