
using UnityEngine;

public interface IObserver<T>
{
    public abstract void OnNotify(ref T Data);
}
