using UnityEngine.Events;
using System.Threading.Tasks;
public class AssetPreloadEvent: UnityEventWTAsync<bool, Preloader>
{
    private UnityEvent<bool, Preloader> m_assetPreloadEvent = new UnityEvent<bool, Preloader>();    
    public override UnityEvent<bool, Preloader> GetInstance()
    {
        return m_assetPreloadEvent;
    }
    public override Task AddListener(UnityAction<bool, Preloader> action)
    {
        m_assetPreloadEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(bool tValue, Preloader zValue)
    {
        m_assetPreloadEvent.Invoke(tValue, zValue);

        return Task.CompletedTask;
    }
}