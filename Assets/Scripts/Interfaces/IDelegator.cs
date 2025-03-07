using System.Collections;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, params object[] optional);
    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, params object[] optional);
}

public interface IDelegator<T, Z>
{
    public IEnumerator NotifyObserver(IObserver<T, Z> observer, Z value, NotificationContext notificationContext = null, params object[] optional);
    public IEnumerator NotifySubject(IObserver<T, Z> observer, NotificationContext notificationContext = null, params object[] optional);
}