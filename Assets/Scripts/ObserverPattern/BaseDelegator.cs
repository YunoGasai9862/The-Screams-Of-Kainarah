using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

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
        return subject == null || subject.GetSubject() == null;
    }
}

public abstract class BaseDelegator<T, Z> : MonoBehaviour, IDelegator<T, Z>
{
    private Dictionary<string, Subject<IObserver<T, Z>>> SubjectsDict { get; set; } = new Dictionary<string, Subject<IObserver<T, Z>>>();

    public IEnumerator NotifyObserver(IObserver<T, Z> observer, Z value, NotificationContext notificationContext = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext);

        yield return null;
    }

    public IEnumerator NotifySubject(string key, IObserver<T, Z> observer, NotificationContext notificationContext = null, params object[] optional)
    {
        Subject<IObserver<T,Z>> subject = SubjectsDict[key];

        yield return new WaitUntil(() => !IsSubjectNull(subject));

        subject.NotifySubject(observer, notificationContext);

        yield return null;
    }

    public IEnumerator NotifySubjects(IObserver<T, Z> observer, NotificationContext notificationContext = null, params object[] optional)
    {
        foreach(Subject<IObserver<T,Z>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubject(observer, notificationContext);
        }

        yield return null;
    }

    private bool IsSubjectNull(Subject<IObserver<T, Z>> subject)
    {
        return subject == null || subject.GetSubject() == null;
    }
}
