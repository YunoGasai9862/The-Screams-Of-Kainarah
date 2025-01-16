using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class ObserverQueue<T>
{
    private Queue<IObserver<T>> m_observers = new Queue<IObserver<T>>();

    public Queue<IObserver<T>> ObserverEntities { set => m_observers = value; get => m_observers; }

    public void EnqueueObserver(IObserver<T> observer)
    {
        m_observers.Enqueue(observer);
    }

    public void NotifyObservers(T value, SemaphoreSlim lockingThread = null) //good to empty, so we dont notify the same observers again
    {
        while (m_observers.Count > 0)
        {
            IObserver<T> observer = m_observers.Dequeue();

            observer.OnNotify(value, lockingThread);
        }
    }
}

public class ObserverList<T>
{

    private List<IObserver<T>> _potentialObservers = new();

    public List<IObserver<T>> potentialObservers { set => _potentialObservers = value; get => _potentialObservers; }

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

public class ObserverExtended<T, Y, Z>
{
    private List<IExtendedObserver<T, Y, Z>> _potentialObservers = new();

    public List<IExtendedObserver<T, Y, Z>> potentialObservers { set => _potentialObservers = value; get => _potentialObservers; }

    public void AddObserver(IExtendedObserver<T, Y, Z> observer)
    {
        _potentialObservers.Add(observer);
    }

    public void RemoveOberver(IExtendedObserver<T, Y, Z> observer)
    {
        _potentialObservers.Remove(observer);
    }
    public void NotifyObservers(T objectCausingTrigger, Z dataTypeValue, Y dataTypeValue2)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(objectCausingTrigger, dataTypeValue, dataTypeValue2);
        }
    }

}
