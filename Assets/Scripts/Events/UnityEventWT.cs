using UnityEngine.Events;

//with type <T>
public class UnityEventWT<T> : UnityEvent<T> //extends the base class, but adds GetInstance functionality
{
    private static UnityEventWT<T> _event;

    private UnityEventWT() { }

    public static UnityEventWT<T> GetInstance()
    {
        if(_event == null)
        {
            _event = new UnityEventWT<T>();
        }
        return _event;
    }
}