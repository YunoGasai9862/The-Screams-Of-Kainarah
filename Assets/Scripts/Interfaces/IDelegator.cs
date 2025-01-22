using System.Collections;
public interface IDelegator<T>
{
    public IEnumerator NotifyObserver(IObserver<T> observer, T value);
    public IEnumerator NotifySubject(IObserver<T> observer);
}