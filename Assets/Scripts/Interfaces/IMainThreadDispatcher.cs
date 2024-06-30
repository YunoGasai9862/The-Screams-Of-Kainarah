using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IMainThreadDispatcher
{
    abstract Task Dispatcher(Action action, CancellationToken cancellationToken);

}