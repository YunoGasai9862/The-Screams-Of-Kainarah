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

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null,params object[] optional)
    {
        if (ObserverSubjectDict.TryGetValue(observer.GetType().ToString(), out List<ObserverSystemAttribute> attributes))
        {
            if (notificationContext.SubjectType == null)
            {
                throw new ApplicationException($"Subject type is null - please add it in the notification context object!");
            }

            ObserverSystemAttribute targetObserverSystemAttribute = GetTargetObserverSystemAttribute(notificationContext.SubjectType, attributes);

            Subject<IObserver<T>> subject = SubjectsDict[targetObserverSystemAttribute.SubjectType.ToString()];

            yield return new WaitUntil(() => !IsSubjectNull(subject));

            subject.NotifySubject(observer, notificationContext);
        }

        yield return null;
    }

    private bool IsSubjectNull(Subject<IObserver<T>> subject)
    {
        return subject == null || subject.GetSubject() == null;
    }

    private ObserverSystemAttribute GetTargetObserverSystemAttribute(string subjectType, List<ObserverSystemAttribute> attributes)
    {
        foreach(ObserverSystemAttribute attribute in attributes)
        {
            if (string.CompareOrdinal(subjectType, attribute.SubjectType.ToString()) == 0)
            {
                return attribute;
            }
        }

        return null;
    }
 }
