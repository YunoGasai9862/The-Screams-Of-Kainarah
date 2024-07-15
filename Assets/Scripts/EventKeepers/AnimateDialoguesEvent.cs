using UnityEngine.Events;
using System.Threading.Tasks;
public class AnimationDialoguesEvent: UnityEventWTAsync<bool>
{
    public override UnityEvent<bool> GetInstance()
    {
        return null;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        return null;

    }
    public override Task Invoke(bool value)
    {
        return null;

    }
}