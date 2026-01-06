using System.Collections;
using System.Threading;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional);

    public IEnumerator NotifySubject(IObserver<T> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 1000, params object[] optional);
}

public interface IDelegator
{
    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional);

    public IEnumerator NotifySubject<T>(ObserverContext<T> context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional);

    public void BuildRegistry();
}