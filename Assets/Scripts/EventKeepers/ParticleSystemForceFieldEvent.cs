using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemForceFieldEvent : UnityEventWTAsync<ParticleSystemForceField>
{
    public override Task AddListener(UnityAction<ParticleSystemForceField> action)
    {
        throw new System.NotImplementedException();
    }

    public override UnityEvent<ParticleSystemForceField> GetInstance()
    {
        throw new System.NotImplementedException();
    }

    public override Task Invoke(ParticleSystemForceField value)
    {
        throw new System.NotImplementedException();
    }
}