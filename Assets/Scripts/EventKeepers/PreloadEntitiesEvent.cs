using System.Threading.Tasks;
using UnityEngine.Events;

public class PreloadEntitiesEvent : UnityEventWTAsync<PreloadEntity[]>
{

    private UnityEvent<PreloadEntity[]> m_preloadEntities = new UnityEvent<PreloadEntity[]>();
    public override Task AddListener(UnityAction<PreloadEntity[]> action)
    {
        m_preloadEntities.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<PreloadEntity[]> GetInstance()
    {
        return m_preloadEntities;
    }

    public override Task Invoke(PreloadEntity[] value)
    {
        m_preloadEntities.Invoke(value);    

        return Task.CompletedTask;
    }
}