using System.Collections;
using System.Threading;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null,params object[] optional);

}

public interface IDelegatorEnhanced<T>
{
    public IEnumerator NotifyObserver(IObserverEnhanced<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubject(string key, IObserverEnhanced<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubjects(IObserverEnhanced<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifyObserver(IObserverEnhanced<T> observer, string key, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifyWhenActive(IObserverEnhanced<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
}
