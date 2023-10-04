
public interface IObserver<T>
{
    public abstract void OnNotify(ref T Data, params object[] optional);

}
public interface IObserver<T,I> //simply create another interface instead of inheriting from the previous one
{
    public abstract void OnNotify(ref T Data, params I[] optional);
}
