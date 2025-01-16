using System.Threading;
using System.Threading.Tasks;

public class SubjectAsync<T>
{
    private ISubjectAsync<T> MSubject {  get; set; }   
    public void SetSubject(ISubjectAsync<T> subject)
    {
        MSubject = subject; 
    }

    public async Task NotifySubject(T value, SemaphoreSlim lockingThread = null)
    {
       await MSubject.OnNotifySubject(value, lockingThread);
    }
}


public class SubjectAsync
{
    private ISubjectAsync MSubject { get; set; }
    public void SetSubject(ISubjectAsync subject)
    {
        MSubject = subject;
    }

    public async Task NotifySubject(SemaphoreSlim lockingThread = null)
    {
        await MSubject.OnNotifySubject(lockingThread);
    }
}