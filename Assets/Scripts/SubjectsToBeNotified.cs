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

public class SubjectsToBeNotified<T, I> //for player
{
    private List<IObserver<T, I>> _potentialObservers = new();

    public List<IObserver<T, I>> potentialObservers { set => _potentialObservers = value; get => _potentialObservers; }

    public void AddObserver(IObserver<T, I> observer)
    {
        _potentialObservers.Add(observer);
    }

    public void RemoveOberver(IObserver<T, I> observer)
    {
        _potentialObservers.Remove(observer);
    }
    public void NotifyObservers(ref T value, params I[] optional)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(ref value, optional);
        }
    }

}

