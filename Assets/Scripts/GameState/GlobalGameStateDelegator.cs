
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GlobalGameStateDelegator: BaseDelegatorEnhanced<GenericState<GameStateConsumer>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericState<GameStateConsumer>>>>>();
    }

    public void NotifySubjectWrapper(IObserver<GenericState<GameStateConsumer>> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}