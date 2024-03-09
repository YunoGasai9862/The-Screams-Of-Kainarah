
using UnityEngine.Events;

public class ProjectileThrowAnimationEvent: UnityEventWOT
{
    private static UnityEvent _instance = new UnityEvent();
    public ProjectileThrowAnimationEvent() { }

    public override UnityEvent GetInstance()
    {
        return _instance;
    }
    public static void AddEventListener(UnityAction value)
    {
        _instance.AddListener(value);
    }

}