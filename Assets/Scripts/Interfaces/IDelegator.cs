using System.Collections;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null);
    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null);
}