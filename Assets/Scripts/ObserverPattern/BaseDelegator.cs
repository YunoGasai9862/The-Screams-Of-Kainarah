using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

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
    protected Dictionary<string, Dictionary<string, Subject<IObserver<T>>>> SubjectsDict { get; set; }

    protected Dictionary<string, List<Association<T>>> SubjectObserversDict { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        observer.OnNotify(value, notificationContext, semaphoreSlim, cancellationToken, optional);

        yield return null;
    }

    //later convert it to for-loop based retry method
    public IEnumerator NotifySubject(IObserver<T> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        if (maxRetries == 0)
        {
            throw new ApplicationException($"No such subject type exists! - Please Register first {notificationContext.SubjectType}");
        }

        if (notificationContext.SubjectType == null)
        {
            throw new ApplicationException($"Subject type is null - please add it in the notification context object!");
        }

        yield return new WaitUntil(() => !Helper.IsObjectNull(SubjectsDict));

        if (SubjectsDict.TryGetValue(notificationContext.SubjectType, out Dictionary<string, Subject<IObserver<T>>> subjects))
        {
            foreach(KeyValuePair<string, Subject<IObserver<T>>> keyValuePair in subjects)
            {
                yield return new WaitUntil(() => !Helper.IsSubjectNull(keyValuePair.Value));

                keyValuePair.Value.NotifySubject(observer, notificationContext, cancellationToken);
            }
        }
        else
        {
            yield return new WaitForSeconds(Helper.GetSecondsFromMilliSeconds(sleepTimeInMilliSeconds));

            StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries -= 1, sleepTimeInMilliSeconds, optional));
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

    public void AddToSubjectObserversDict(string uniqueSubjectkey, Subject<IObserver<T>> subject, IObserver<T> observer)
    {
        if (SubjectObserversDict.ContainsKey(uniqueSubjectkey))
        {
            SubjectObserversDict[uniqueSubjectkey].Add(new Association<T>() { Observer = observer, Subject = subject });

            return;
        }

        SubjectObserversDict.Add(uniqueSubjectkey, new List<Association<T>>()
        { new Association <T>() { Observer = observer, Subject = subject } });
    }

    public Dictionary<string, List<Association<T>>> GetSubjectObserversDict()
    {
        return SubjectObserversDict;
    }

    public List<Association<T>> GetSubjectAssociations(string subjectUniqueKey)
    {
        return SubjectObserversDict[subjectUniqueKey];
    }

    public Dictionary<string, Subject<IObserver<T>>> GetSubsetSubjectsDictionary(string key)
    {
        if (SubjectsDict.TryGetValue(key, out Dictionary<string, Subject<IObserver<T>>> subject))
        {
            return subject;
        }

        return null;
    }

    public void NotifyObservers(T valueToSend, string subjectUniqueKey, Type subjectType, CancellationToken cancellationToken)
    {
        foreach (Association<T> association in GetSubjectAssociations(subjectUniqueKey))
        {
            StartCoroutine(NotifyObserver(association.Observer, valueToSend, new NotificationContext()
            {
                SubjectType = subjectType.ToString()

            }, cancellationToken));
        }
    }
}
