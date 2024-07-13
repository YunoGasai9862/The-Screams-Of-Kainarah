using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

//with type <T>
public abstract class UnityEventWTAsync<T> : MonoBehaviour, ICustomUnityEventWTAsync<T> 
{
    public abstract UnityEvent<T> GetInstance();
    public abstract Task AddListener(UnityAction<T> action);
    public abstract Task Invoke(T value);
}

public abstract class UnityEventWTAsync<T, Z> : MonoBehaviour, ICustomUnityEventWTAsync<T, Z>
{
    public abstract UnityEvent<T, Z> GetInstance();
    public abstract Task AddListener(UnityAction<T, Z> action);
    public abstract Task Invoke(T tValue, Z zValue);
}

public abstract class UnityEventWT<T>: MonoBehaviour, ICustomUnityEvent<T>
{
    public abstract UnityEvent<T> GetInstance();
    public abstract void AddListener(UnityAction<T> action);
    public abstract void Invoke(T value);
}