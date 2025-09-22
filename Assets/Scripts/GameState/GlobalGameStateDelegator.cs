
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

public class GlobalGameStateDelegator: BaseDelegator<GenericStateBundle<GameStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<GameStateBundle>>>>>();
    }

    public void NotifySubjectWrapper(IObserver<GenericStateBundle<GameStateBundle>> observer, NotificationContext notificationContext, 
        CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, 
        int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        Debug.Log($"NotifySubjectWrapper before - for {observer} - and the count: {SubjectsDict.Count}");
        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
        Debug.Log($"NotifySubjectWrapper after - for {observer} - and the count: {SubjectsDict.Count}");
    }
}