using UnityEngine.Events;

//without type <T>
public abstract class UnityEventWOT : UnityEvent //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEventWOT GetInstance();
}