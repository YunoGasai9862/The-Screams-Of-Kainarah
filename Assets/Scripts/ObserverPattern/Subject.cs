using System.Threading;

public class Subject<T>
{
    private ISubjectAsync<T> MSubject {  get; set; }   
    public void SetSubject(ISubjectAsync<T> subject)
    {
        MSubject = subject; 
    }

    public void NotifySubject(T value, SemaphoreSlim lockingThread = null)
    {
        MSubject.OnNotifySubject(value, lockingThread);
    }
}