using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SubjectsToBeNotified<T> //for player
{

    private List<IObserver<T>> _potentialObservers = new();

    public List<IObserver<T>> potentialObservers { set=>_potentialObservers=value; get=>_potentialObservers; }

    public void AddObserver(IObserver<T> observer)
    {
        _potentialObservers.Add(observer);
    }

    public void RemoveOberver(IObserver<T> observer)
    {
        _potentialObservers.Remove(observer);
    }
    public void NotifyObservers(T value, SemaphoreSlim lockingThread = null)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(value, lockingThread);
        }
    }

}

public class SubjectsToBeNotifiedV2<T> //for player
{
    private List<IObserverV2<T>> _potentialObservers = new();

    public List<IObserverV2<T>> potentialObservers { set => _potentialObservers = value; get => _potentialObservers; }

    public void AddObserver(IObserverV2<T> observer)
    {
        _potentialObservers.Add(observer);
    }

    public void RemoveOberver(IObserverV2<T> observer)
    {
        _potentialObservers.Remove(observer);
    }
    public void NotifyObservers<Z, Y>(T objectCausingTrigger, Z dataTypeValue, Y dataTypeValue2)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(objectCausingTrigger, dataTypeValue, dataTypeValue2);
        }
    }

}

