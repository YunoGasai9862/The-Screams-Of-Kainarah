using System.Collections;

public interface INotify
{
    public IEnumerator Notify(object value);
}

public interface INotify<T>
{
    public IEnumerator Notify(T value);
}
