using System.Threading.Tasks;
using UnityEngine.Events;

public class DialogueConcludedEvent : UnityEventWTAsync<bool>
{
    public override Task AddListener(UnityAction<bool> action)
    {
        throw new System.NotImplementedException();
    }

    public override UnityEvent<bool> GetInstance()
    {
        throw new System.NotImplementedException();
    }

    public override Task Invoke(bool value)
    {
        throw new System.NotImplementedException();
    }
}