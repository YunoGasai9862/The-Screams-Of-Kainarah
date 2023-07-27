
using UnityEngine;

public interface IObserver<T>
{
    public abstract void OnNotify(T Data);
}
