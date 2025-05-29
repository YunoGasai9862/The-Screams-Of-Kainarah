
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GlobalGameStateDelegator: BaseDelegatorEnhanced<GameState>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GameState>>>>();
    }

    public void NotifySubjectWrapper(IObserver<GameState> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}