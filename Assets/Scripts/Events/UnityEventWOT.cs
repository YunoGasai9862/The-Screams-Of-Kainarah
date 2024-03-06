using UnityEngine.Events;

//without type <T>
public class UnityEventWOT : UnityEvent //extends the base class, but adds GetInstance functionality
{
    private static UnityEventWOT _event;
    public UnityEventWOT() { }
    public static UnityEventWOT GetInstance()
    {
        if (_event == null)
        {
            _event = new UnityEventWOT();
        }
        return _event;
    }
}