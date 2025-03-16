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

public abstract class BaseDelegator<T, Z> : IDelegator<T, Z> where T: MonoBehaviour
{
    public Dictionary<string, SubjectNotifier<IObserver<T, Z>>> SubjectsDict { get; set; }

    public IEnumerator NotifyObserver(IObserver<T, Z> observer, Z value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(string key, IObserver<T, Z> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null,params object[] optional)
    {
        SubjectNotifier<IObserver<T,Z>> subject = SubjectsDict[key];

        yield return new WaitUntil(() => !IsSubjectNull(subject));

        subject.NotifySubject(observer, notificationContext);

        yield return null;
    }

    public IEnumerator NotifySubjects(IObserver<T, Z> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        foreach(SubjectNotifier<IObserver<T,Z>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubject(observer, notificationContext);
        }

        yield return null;
    }

    public IEnumerator NotifyObserver(IObserver<T, Z> observer, string key, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnKeyNotify(key, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    private bool IsSubjectNull(SubjectNotifier<IObserver<T, Z>> subject)
    {
        return subject == null || subject.GetSubject() == null;
    }

    public IEnumerator NotifyWhenActive(IObserver<T, Z> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        foreach (SubjectNotifier<IObserver<T, Z>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubjectOfActivation(observer, notificationContext);
        }

        yield return null;
    }
}
