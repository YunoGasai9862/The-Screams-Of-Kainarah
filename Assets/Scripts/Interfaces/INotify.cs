using System.Threading.Tasks;

public interface INotify<T>
{
    public Task Notify(T value);
}