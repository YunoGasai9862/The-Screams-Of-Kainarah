using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PreloadedEntitiesEvent : UnityEventWTAsync<List<UnityEngine.Object>>
{

    private UnityEvent<List<UnityEngine.Object>> m_preloadedEntities = new UnityEvent<List<UnityEngine.Object>>();
    public override Task AddListener(UnityAction<List<UnityEngine.Object>> action)
    {
        m_preloadedEntities.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<List<UnityEngine.Object>> GetInstance()
    {
        return m_preloadedEntities;
    }

    public override Task Invoke(List<UnityEngine.Object> entities)
    {
        m_preloadedEntities.Invoke(entities);

        return Task.CompletedTask;
    }
}