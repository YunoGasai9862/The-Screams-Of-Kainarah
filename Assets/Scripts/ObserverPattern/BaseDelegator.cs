using System.Collections;
using UnityEngine;

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegator<T>
{
    public Subject<IObserver<T>> Subject { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null)
    {
        observer.OnNotify(value);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null)
    {
        yield return new WaitUntil(() => Subject.GetSubject() != null);

        Subject.NotifySubject(observer);

        yield return null;
    }
}