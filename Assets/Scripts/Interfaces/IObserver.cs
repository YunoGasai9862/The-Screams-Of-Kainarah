
public interface IObserver<T>
{
    public abstract void OnNotify(T Data, params object[] optional);

}
public interface IObserverV2<T>
{
    public abstract void OnNotify<Z, Y>(T Data, Z value1, Y value2);
}
