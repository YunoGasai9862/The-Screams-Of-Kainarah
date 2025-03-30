using System.Collections;
using System.Threading;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null, params object[] optional);
    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null,params object[] optional);

}