using System.Threading.Tasks;
public interface IDelegatorAsync<T>
{
    public Task NotifyObserver(IObserverAsync<T> observer, T value);
    public Task NotifySubject(IObserverAsync<T> observer);
}