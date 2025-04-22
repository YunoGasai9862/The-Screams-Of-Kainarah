using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using Amazon.Runtime.Internal.Transform;

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegator<T>
{
    public Subject<IObserver<T>> Subject { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, cancellationToken, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        yield return new WaitUntil(() => !Helper.IsSubjectNull(Subject));

        Subject.NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, optional);

        yield return null;
    }
}


public abstract class BaseDelegatorEnhanced<T> : MonoBehaviour, IDelegator<T>
{
    //support for multiple subjects per script/subject type (same script can be attached to multiple game objects)
    protected Dictionary<string, Dictionary<string, Subject<IObserver<T>>>> SubjectsDict { get; set; }
    protected Dictionary<string, List<ObserverSystemAttribute>> ObserverSubjectDict { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, cancellationToken, optional);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
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

            ObserverSystemAttribute targetObserverSystemAttribute = GetTargetObserverSystemAttribute(notificationContext.SubjectType, attributes);

            if (SubjectsDict.TryGetValue(targetObserverSystemAttribute.SubjectType.ToString(), out Dictionary<string, Subject<IObserver<T>>> subjects))
            {
                //usually we want to notify all of the instances because the point of having a dictionary is to store multiple game objects utilizing the same script
                //and not cherry pick just one subject - otherwise it defeats the purpose of having a dictionary stored against the script type

                foreach(Subject<IObserver<T>> subject in subjects.Values)
                {
                    yield return new WaitUntil(() => !Helper.IsSubjectNull(subject));

                    subject.NotifySubject(observer, notificationContext, cancellationToken);
                }

            }
            else
            {
                yield return new WaitForSeconds(Helper.GetSecondsFromMilliSeconds(sleepTimeInMilliSeconds));

                StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries -= 1, sleepTimeInMilliSeconds, optional));
            }
        }

        yield return null;
    }

    public void AddToSubjectsDict(string mainSubjectIdentificationKey, string gameObjectInstanceIdentificationKeyForTheSubject, Subject<IObserver<T>> subject)
    {
        if (SubjectsDict.ContainsKey(mainSubjectIdentificationKey))
        {
            Debug.Log($"Key already exists {mainSubjectIdentificationKey}. Won't persist it again!");

            if (SubjectsDict[mainSubjectIdentificationKey].ContainsKey(gameObjectInstanceIdentificationKeyForTheSubject))
            {
                Debug.Log($"Game Object Identifier already exists {gameObjectInstanceIdentificationKeyForTheSubject} for the subject, won't persist it again!");

                return;

            }else
            {
                SubjectsDict[mainSubjectIdentificationKey].Add(gameObjectInstanceIdentificationKeyForTheSubject, subject);
            }

            return;
        }

        SubjectsDict.Add(mainSubjectIdentificationKey, new Dictionary<string, Subject<IObserver<T>>> {

            {gameObjectInstanceIdentificationKeyForTheSubject, subject }
        
        });
    }

    public Dictionary<string, Dictionary<string, Subject<IObserver<T>>>> GetMainSubjectsDictionary()
    {
        return SubjectsDict;
    }

    public Dictionary<string, Subject<IObserver<T>>> GetSubsetSubjectsDictionary(string key)
    {
        if (SubjectsDict.TryGetValue(key, out Dictionary<string, Subject<IObserver<T>>> subject))
        {
            return subject;
        }

        return null;
    }

    protected ObserverSystemAttribute GetTargetObserverSystemAttribute(string subjectType, List<ObserverSystemAttribute> attributes)
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
