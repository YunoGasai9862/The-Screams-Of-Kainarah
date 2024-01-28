public interface ICommand<T>
{
    public abstract void Execute(T value= default);
    public abstract void Cancel(T value= default);
}
