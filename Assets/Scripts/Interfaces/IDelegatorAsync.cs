using System.Threading.Tasks;
public interface IDelegatorAsync<T>
{
    public SubjectAsync<IObserverAsync<T>> Subject { get; } 
    public Task NotifyObserver(IObserverAsync<T> observer, T value);
    public Task NotifySubject(IObserverAsync<T> observer);
}