using System.Collections;
using System.Collections.Generic;
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

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return new WaitUntil(() => !Helper.IsSubjectNull(Subject));

        Subject.NotifySubject(observer, notificationContext, semaphoreSlim, optional);

        yield return null;
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

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        if (maxRetries == 0)
        {
            throw new ApplicationException($"No such subject type exists! - Please Register first {SubjectsDict.Count}");
        }

        if (ObserverSubjectDict.TryGetValue(observer.GetType().ToString(), out List<ObserverSystemAttribute> attributes))
        {
            if (notificationContext.SubjectType == null)
            {
                throw new ApplicationException($"Subject type is null - please add it in the notification context object!");
            }

            ObserverSystemAttribute targetObserverSystemAttribute = GetTargetObserverSystemAttribute(notificationContext.SubjectType, attributes);

            Subject<IObserver<T>> subject = null;

            try
            {
                subject = SubjectsDict[targetObserverSystemAttribute.SubjectType.ToString()];
            }
            catch (KeyNotFoundException ex)
            {
                Debug.Log($"Subject: {targetObserverSystemAttribute.SubjectType.ToString()} - is not yet registered, retrying again. MaxRetries left: {maxRetries} {SubjectsDict.Count}");
            }
            finally
            {
                //use this logic to set a flag (tomorrow) to block this thread and not another
                //this sleeps on another thread
                StartCoroutine(WaitForSeconds(Helper.GetSecondsFromMilliSeconds(sleepTimeInMilliSeconds)));

                StartCoroutine(NotifySubject(observer, notificationContext, semaphoreSlim, maxRetries -= 1, sleepTimeInMilliSeconds, optional));
            }

            yield return new WaitUntil(() => !Helper.IsSubjectNull(subject));

            subject.NotifySubject(observer, notificationContext);
        }

        yield return null;
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
    
    private IEnumerator WaitForSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
 }
