
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

public class GlobalGameStateDelegator: BaseDelegatorEnhanced<GenericStateBundle<GameStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<GameStateBundle>>>>>();
    }

    public void NotifySubjectWrapper(IObserver<GenericStateBundle<GameStateBundle>> observer, NotificationContext notificationContext, 
        CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, 
        int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        Debug.Log($"Using Wrapper for - {notificationContext.SubjectType} / {observer} - length of the dict :{SubjectsDict.Count}");

        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}