using System.Threading.Tasks;
public interface IDelegator<T>
{
    public Observers<T> Observers { get; set; }
    public Task NotifySubjectsAsync();

    public Task NotifyListenerAsync();
}