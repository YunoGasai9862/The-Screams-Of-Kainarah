using System.Threading.Tasks;
public interface IDelegator
{
    public Task NotifySubjectAsync();

    public Task NotifyListenerAsync();
}