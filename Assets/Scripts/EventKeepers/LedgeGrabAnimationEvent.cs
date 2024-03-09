using UnityEngine.Events;
public class LedgeGrabAnimationEvent : UnityEventWT<bool>
{
    private static UnityEvent<bool> _instance = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        throw new System.NotImplementedException();
    }
    public static void AddEventListener(UnityAction value)
    {
        _instance.AddListener(value);
    }
}