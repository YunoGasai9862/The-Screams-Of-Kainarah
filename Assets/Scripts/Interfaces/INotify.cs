using System.Threading.Tasks;


public interface INotify
{
    public Task Notify(object value);
}

public interface INotify<T>: INotify
{
    public Task Notify(T value);
}
