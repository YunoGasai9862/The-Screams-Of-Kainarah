

public interface IObserver<T>
{
    public abstract void OnNotify(T data, NotificationContext notificationContext, params object[] optional);

}
public interface IExtendedObserver<T, Y, Z>
{
    public abstract void OnNotify(T Data, Z value1, Y value2);
}

/// <summary>
/// Represents a subject for asynchronous observer pattern
/// Allows us to use ping the same type of subject with different values type
/// And helps us differentiate what type of subject we want, and what value we are seeking from that subject
/// <typeparam name="T">The type of Implementation for the subject/typeparam>
/// <typeparam name="Z">The data we want from the subject</typeparam>
/// </summary>
public interface IObserver<T, Z> 
{
    public abstract void OnNotify(Z data, NotificationContext context, params object[] optional);
}