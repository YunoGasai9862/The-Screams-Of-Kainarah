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

//TODO - Get Class Type from observer and then use reflection to grab annotation and ping that subject only!!!
//new approach use reflection to generate one time dictionary since observers can have a single subject etc
//store in a class instead or an array to have a list of subjects instead

public abstract class BaseDelegatorEnhanced<T> : MonoBehaviour, IDelegatorEnhanced<T>
{
    public Dictionary<string, SubjectNotifier<IObserverEnhanced<T>>> SubjectsDict { get; set; }

    public IEnumerator NotifyObserver(IObserverEnhanced<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(string key, IObserverEnhanced<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null,params object[] optional)
    {
        SubjectNotifier<IObserverEnhanced<T>> subject = SubjectsDict[key];

        yield return new WaitUntil(() => !IsSubjectNull(subject));

        subject.NotifySubject(observer, notificationContext);

        yield return null;
    }

    public IEnumerator NotifySubjects(IObserverEnhanced<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        foreach(SubjectNotifier<IObserverEnhanced<T>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubject(observer, notificationContext);
        }

        yield return null;
    }

    public IEnumerator NotifyObserver(IObserverEnhanced<T> observer, string key, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnKeyNotify(key, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    private bool IsSubjectNull(SubjectNotifier<IObserverEnhanced<T>> subject)
    {
        return subject == null || subject.GetSubject() == null;
    }

    public IEnumerator NotifyWhenActive(IObserverEnhanced<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        Debug.Log($"Here!! {observer}");

        foreach (SubjectNotifier<IObserverEnhanced<T>> subject in SubjectsDict.Values)
        {
            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubjectOfActivation(observer, notificationContext);
        }

        yield return null;
    }
}
