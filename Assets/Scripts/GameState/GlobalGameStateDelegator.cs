
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

public class GlobalGameStateDelegator: BaseDelegatorEnhanced<GenericState<GameState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericState<GameState>>>>>();
    }

    public void NotifySubjectWrapper(IObserver<GenericState<GameState>> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}