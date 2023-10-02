

public interface IObserver<T,I>
{
    public abstract void OnNotify(ref T Data, params I[] optional);
}
