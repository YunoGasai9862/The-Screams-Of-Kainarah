using System.Threading.Tasks;
using UnityEngine.Events;

public interface ICustomUnityEventWTAsync<T>
{
    UnityEvent<T> GetInstance();
    Task Invoke(T value);
}

public interface ICustomUnityEventWTAsync<T, Z>
{
    UnityEvent<T, Z> GetInstance();
    Task Invoke(T tValue, Z zValue);
}

 public interface ICustomUnityEvent<T>
{
    UnityEvent<T> GetInstance();
    void Invoke(T value);   
}