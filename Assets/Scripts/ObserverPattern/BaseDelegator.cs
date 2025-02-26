using System.Collections;
using UnityEngine;

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegator<T>
{
    public Subject<IObserver<T>> Subject { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, params object[] optional)
    {
        yield return new WaitUntil(() => !IsSubjectNull(Subject));

        Subject.NotifySubject(observer, notificationContext);

        yield return null;
    }

    private bool IsSubjectNull(Subject<IObserver<T>> subject)
    {
        return Subject == null || Subject.GetSubject() == null;
    }
}