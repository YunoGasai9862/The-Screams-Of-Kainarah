using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional);

    public IEnumerator NotifySubject(IObserver<T> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional);
}

public interface IDelegator
{
    IEnumerator NotifyObservers<T>(SubjectContext<T> context, IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional);

    IEnumerator NotifyObserver<T>(SubjectContext<T> context, IRequest<T> subject, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional);

    IEnumerator NotifySubject<T>(ObserverContext<T> context, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional);

    Task<Dictionary<dynamic, List<dynamic>>> BuildDelegatorRegistry();
}