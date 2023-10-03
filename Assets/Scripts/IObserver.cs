
public interface IObserver<T>
{
    public abstract void OnNotify(ref T Data, params object[] optional);

}
public interface IObserver<T,I>: IObserver<T> //this is a better approach implements the base class with a generic type
{
    public abstract void OnNotify(ref T Data, params I[] optional);
}
