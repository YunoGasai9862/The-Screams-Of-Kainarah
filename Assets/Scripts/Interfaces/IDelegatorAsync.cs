using System.Threading.Tasks;
public interface IDelegatorAsync<T>
{
    public ObserverQueue<T> Observers { get; }
    public Subject Subject { get; } 
    public Task NotifyObservers();
    public Task NotifySubject();
}