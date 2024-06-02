using System.Threading.Tasks;
using UnityEngine.Events;

public interface ICustomUnityEventWT<T>
{
    UnityEvent<T> GetInstance();
    Task Invoke(T value);
}

public interface ICustomUnityEventWT<T, Z>
{
    UnityEvent<T, Z> GetInstance();
    Task Invoke(T tValue, Z zValue);
}