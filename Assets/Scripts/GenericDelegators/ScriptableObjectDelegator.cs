using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ScriptableObjectDelegator : BaseDelegator<ScriptableObject>
{
    public void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<ScriptableObject>>>();
    }

    public void NotifySubjectWrapper(IObserver<ScriptableObject> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifySubject(observer, context, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }

    public void NotifyObjectWrapper(IObserver<ScriptableObject> observer, ScriptableObject value, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifyObserver(observer, value, context, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}