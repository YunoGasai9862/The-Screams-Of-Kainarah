using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IMainThreadDispatcher
{
    abstract Task Dispatcher(Action<object> action, object parameter = null, CancellationToken cancellationToken);

}