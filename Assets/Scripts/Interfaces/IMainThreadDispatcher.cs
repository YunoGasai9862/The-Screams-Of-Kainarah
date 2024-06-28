using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IMainThreadDispatcher
{
    abstract Task Enqueue(Action action, CancellationToken cancellationToken);

    abstract Task Dispatcher(SemaphoreSlim dispatcherSlim, Queue<Action> actionQueue);

}