using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegator<T>
{
    public Subject<IObserver<T>> Subject { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        yield return new WaitUntil(() => !IsSubjectNull(Subject));

        Subject.NotifySubject(observer, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    private bool IsSubjectNull(Subject<IObserver<T>> subject)
    {
        return subject == null || subject.GetSubject() == null;
    }
}


public abstract class BaseDelegatorEnhanced<T> : MonoBehaviour, IDelegator<T>
{
    public Dictionary<string, Subject<IObserver<T>>> SubjectsDict { get; set; }
    public Dictionary<string, List<ObserverSystemAttribute>> ObserverSubjectDict { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null,params object[] optional)
    {
        //do it in other way
        //Subject<IObserver<T>> subject = SubjectsDict[key];

        //yield return new WaitUntil(() => !IsSubjectNull(subject));

        //subject.NotifySubject(observer, notificationContext);

        yield return null;
    }

    public IEnumerator NotifySubjects(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        foreach(Subject<IObserver<T>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubject(observer, notificationContext);
        }

        yield return null;
    }

    private bool IsSubjectNull(Subject<IObserver<T>> subject)
    {
        return subject == null || subject.GetSubject() == null;
    }

    public IEnumerator NotifyWhenActive(IObserver<T> observer, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        Debug.Log($"Here!! {notificationContext.ToString()}");

        foreach (Subject<IObserver<T>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            //subject.NotifySubjectOfActivation(observer, notificationContext);
        }

        yield return null;
    }
}
