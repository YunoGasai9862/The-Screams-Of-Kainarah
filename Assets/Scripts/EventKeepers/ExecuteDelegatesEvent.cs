using System.Threading.Tasks;
using UnityEngine.Events;

public class ExecuteDelegatesEvent : UnityEventWTAsync<PreloadEntity[]>
{
    public override Task AddListener(UnityAction<PreloadEntity[]> action)
    {
        throw new System.NotImplementedException();
    }

    public override UnityEvent<PreloadEntity[]> GetInstance()
    {
        throw new System.NotImplementedException();
    }

    public override Task Invoke(PreloadEntity[] value)
    {
        throw new System.NotImplementedException();
    }
}