using UnityEngine.Events;

//with type <T>
public abstract class UnityEventWT<T> : UnityEvent<T> //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEventWT<T> GetInstance();
}