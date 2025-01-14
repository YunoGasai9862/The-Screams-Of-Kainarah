using System.Collections.Generic;
using System.Threading;

public class Observers<T>
{
    private Queue<IObserver<T>> m_observers = new Queue<IObserver<T>>();

    public Queue<IObserver<T>> ObserverEntities { set => m_observers = value; get => m_observers; }

    public void EnqueueSubject(IObserver<T> observer)
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