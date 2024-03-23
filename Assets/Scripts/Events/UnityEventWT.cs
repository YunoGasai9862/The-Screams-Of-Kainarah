using UnityEngine;
using UnityEngine.Events;

//with type <T>
public abstract class UnityEventWT<T> : MonoBehaviour, ICustomUnityEventWT<T> //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEvent<T> GetInstance();
    public virtual void AddListener(UnityAction<T> action) { }
}

public abstract class UnityEventWT<T, Z> : MonoBehaviour, ICustomUnityEventWT<T, Z>
{
    public abstract UnityEvent<T, Z> GetInstance();
    public virtual void AddListener(UnityAction<T, Z> action) { }
}