using System.Threading.Tasks;

public interface ICommand<T>
{
    public abstract void Execute(T value= default);
    public abstract void Cancel(T value= default);
}

public interface ICommand<T, Z>
{
    public abstract Z Execute(T value = default);
    public abstract Z Cancel(T value = default);
}

public interface ICommandAsyncEnhanced<T, Z>
{
    public System.Type GetExecutingType();
    public abstract Task<ActionExecuted<Z>> Execute(Z value = default);
    public abstract Task<ActionExecuted<Z>> Cancel(Z value = default);
}
