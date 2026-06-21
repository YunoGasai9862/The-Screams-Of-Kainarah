using Assets.Scripts.Scene;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

//with type <T>
public abstract class UnityEventWTAsync<T> : Scene, ICustomUnityEventWTAsync<T> 
{
    public abstract UnityEvent<T> GetInstance();
    public abstract Task AddListener(UnityAction<T> action);
    public abstract Task Invoke(T value);
}

public abstract class UnityEventWTAsync : Scene, ICustomUnityEventWTAsync
{
    public abstract UnityEvent GetInstance();
    public abstract Task AddListener(UnityAction action);
    public abstract Task Invoke();
}

public abstract class UnityEventWTAsync<T, Z> : Scene, ICustomUnityEventWTAsync<T, Z>
{
    public abstract UnityEvent<T, Z> GetInstance();
    public abstract Task AddListener(UnityAction<T, Z> action);
    public abstract Task Invoke(T tValue, Z zValue);
}
//max - instead use classes then
public abstract class UnityEventWTAsync<X, Y, Z> : Scene, ICustomUnityEventWTAsync<X, Y, Z>
{
    public abstract UnityEvent<X, Y, Z> GetInstance();
    public abstract Task AddListener(UnityAction<X, Y, Z> action);
    public abstract Task Invoke(X xValue, Y yValue, Z zValue);
}

public abstract class UnityEventWT<T>: Scene, ICustomUnityEvent<T>
{
    public abstract UnityEvent<T> GetInstance();
    public abstract void AddListener(UnityAction<T> action);
    public abstract void Invoke(T value);
}