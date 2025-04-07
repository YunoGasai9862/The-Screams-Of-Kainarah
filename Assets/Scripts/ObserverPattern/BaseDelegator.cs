using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

//deprecate this soon!

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegator<T>
{
    public Subject<IObserver<T>> Subject { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext = null, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        yield return new WaitUntil(() => !Helper.IsSubjectNull(Subject));

        Subject.NotifySubject(observer, notificationContext, semaphoreSlim, optional);

        yield return null;
    }
}


public abstract class BaseDelegatorEnhanced<T> : MonoBehaviour, IDelegator<T>
{
    protected Dictionary<string, Subject<IObserver<T>>> SubjectsDict { get; set; }
    protected Dictionary<string, List<ObserverSystemAttribute>> ObserverSubjectDict { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        if (maxRetries == 0)
        {
            throw new ApplicationException($"No such subject type exists! - Please Register first {notificationContext.SubjectType}");
        }

        yield return new WaitUntil(() => !Helper.IsObjectNull(ObserverSubjectDict));

        if (ObserverSubjectDict.TryGetValue(observer.GetType().ToString(), out List<ObserverSystemAttribute> attributes))
        {
            if (notificationContext.SubjectType == null)
            {
                throw new ApplicationException($"Subject type is null - please add it in the notification context object!");
            }

            Debug.Log($"Observer here: {observer}");

            ObserverSystemAttribute targetObserverSystemAttribute = GetTargetObserverSystemAttribute(notificationContext.SubjectType, attributes);

            //found the issue, there's a mismatch in SubjectType.ToString() compared to the tags we are setting in the classes! Fix this
            if (SubjectsDict.TryGetValue(targetObserverSystemAttribute.SubjectType.ToString(), out Subject<IObserver<T>> subject))
            {
                Debug.Log($"Inside for this: {targetObserverSystemAttribute}");

                yield return new WaitUntil(() => !Helper.IsSubjectNull(subject));

                subject.NotifySubject(observer, notificationContext);
            }
            else
            {
                yield return new WaitForSeconds(Helper.GetSecondsFromMilliSeconds(sleepTimeInMilliSeconds));

                StartCoroutine(NotifySubject(observer, notificationContext, semaphoreSlim, maxRetries -= 1, sleepTimeInMilliSeconds, optional));
            }
        }

        yield return null;
    }

    public void AddToSubjectsDict(string key, Subject<IObserver<T>> value)
    {
        if (SubjectsDict.ContainsKey(key))
        {
            Debug.Log($"Key already exists {key}. Won't persist again!");

            return;
        }

        SubjectsDict.Add(key, value);
    }

    public Dictionary<string, Subject<IObserver<T>>> GetSubjectsDict()
    {
        return SubjectsDict;
    }

    public Subject<IObserver<T>> GetSubject(string key)
    {
        if (SubjectsDict.TryGetValue(key, out Subject<IObserver<T>> subject))
        {
            return subject;
        }

        return null;
    }

    protected ObserverSystemAttribute GetTargetObserverSystemAttribute(string subjectType, List<ObserverSystemAttribute> attributes)
    {
        foreach(ObserverSystemAttribute attribute in attributes)
        {
            Debug.Log($"attribute: {attribute} - trying to find {subjectType}");

            if (string.CompareOrdinal(subjectType, attribute.SubjectType.ToString()) == 0)
            {
                Debug.Log($"Found it: {attribute.SubjectType.ToString()} - trying to find {subjectType}");

                return attribute;
            }
        }

        return null;
    }
 }
