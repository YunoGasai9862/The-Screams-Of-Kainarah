using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ScriptableObjectDelegator : BaseDelegatorEnhanced<ScriptableObject>
{
    public void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<ScriptableObject>>>>();
    }

    public void NotifySubjectWrapper(IObserver<ScriptableObject> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }

    public void NotifyObjectWrapper(IObserver<ScriptableObject> observer, ScriptableObject value, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifyObserver(observer, value, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}