
using System.Threading;
using System.Threading.Tasks;

public interface IObserverAsync<T>
{
    public abstract Task OnNotify(T data, CancellationToken token);
}
