using System.Collections.Generic;
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
    public void NotifyObservers(ref T value)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(ref value);
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
    public void NotifyObservers<Z>(ref T value, Z value2)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(ref value, value2);
        }
    }

}

