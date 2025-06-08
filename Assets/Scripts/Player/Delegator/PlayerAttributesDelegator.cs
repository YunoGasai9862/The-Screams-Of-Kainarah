using System.Collections.Generic;
using System.Threading;

public class PlayerAttributesDelegator : BaseDelegatorEnhanced<Player>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<Player>>>>();
    }

    public void NotifySubjectWrapper(IObserver<Player> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional)
    {
        StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries, sleepTimeInMilliSeconds));
    }
}