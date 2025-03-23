using System.Collections;
using System.Threading;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null,params object[] optional);
}

public interface IDelegator<T, Z>
{
    public IEnumerator NotifyObserver(IObserver<T, Z> observer, Z value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubject(string key, IObserver<T, Z> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubjects(IObserver<T, Z> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifyObserver(IObserver<T, Z> observer, string key, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifyWhenActive(IObserver<T, Z> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional);
}
