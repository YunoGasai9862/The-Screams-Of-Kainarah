using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemForceFieldEvent : UnityEventWT<ParticleSystemForceField>
{
    public override Task AddListener(UnityAction<ParticleSystemForceField> action)
    {
        throw new System.NotImplementedException();
    }

    public override UnityEvent<ParticleSystemForceField> GetInstance()
    {
        throw new System.NotImplementedException();
    }
}