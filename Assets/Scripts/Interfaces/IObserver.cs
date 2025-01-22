

public interface IObserver<T>
{
    public abstract void OnNotify(T data, params object[] optional);

}
public interface IExtendedObserver<T, Y, Z>
{
    public abstract void OnNotify(T Data, Z value1, Y value2);
}