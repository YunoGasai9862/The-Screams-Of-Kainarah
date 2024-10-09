using System.Threading.Tasks;
using UnityEngine.Events;

public interface ICustomUnityEventWTAsync<T>
{
    public UnityEvent<T> GetInstance();
    public Task Invoke(T value);
}

public interface ICustomUnityEventWTAsync
{
    public UnityEvent GetInstance();
    public Task Invoke();
}

public interface ICustomUnityEventWTAsync<T, Z>
{
    public UnityEvent<T, Z> GetInstance();
    public Task Invoke(T tValue, Z zValue);
}

public interface ICustomUnityEventWTAsync<X, Y, Z>
{
    public UnityEvent<X, Y, Z> GetInstance();
    public Task Invoke(X xValue, Y yValue, Z zValue);
}


public interface ICustomUnityEvent<T>
{
    public UnityEvent<T> GetInstance();
    public void Invoke(T value);   
}