using System.Threading.Tasks;
using UnityEngine.Events;

public interface ICustomUnityEventWTAsync<T>
{
    public abstract Task AddListener(UnityAction<T> action);
    public UnityEvent<T> GetInstance();
    public Task Invoke(T value);
}

public interface ICustomUnityEventWTAsync
{
    public abstract void AddListener(UnityAction action);
    public UnityEvent GetInstance();
    public Task Invoke();
}

public interface ICustomUnityEventWT
{
    public abstract UnityEvent<dynamic> GetInstance();
    public abstract void AddListener(UnityAction<dynamic> action);
    public abstract void Invoke(dynamic value);
}

public interface ICustomUnityEventWTAsync<T, Z>
{
    Task AddListener(UnityAction<T, Z> action);
    public UnityEvent<T, Z> GetInstance();
    public Task Invoke(T tValue, Z zValue);
}

public interface ICustomUnityEventWTAsync<X, Y, Z>
{
    Task AddListener(UnityAction<X, Y, Z> action);
    public UnityEvent<X, Y, Z> GetInstance();
    public Task Invoke(X xValue, Y yValue, Z zValue);
}

public interface ICustomUnityEvent<T>
{
    void AddListener(UnityAction<T> action);
    UnityEvent<T> GetInstance();
    void Invoke(T value);   
}