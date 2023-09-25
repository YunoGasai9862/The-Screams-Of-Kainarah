using System.Collections.Generic;
public class SubjectsToBeNotified<T>
{
    private List<IObserver<T>> _potentialObservers = new();

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
