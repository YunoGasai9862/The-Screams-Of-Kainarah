using System.Threading;
using System.Threading.Tasks;
using UnityEngine;



/// <summary>
/// Represents a subject for asynchronous observer pattern
/// <typeparam name="T">The type T here is the observer's interface type that the subject notifies</typeparam>
/// </summary>
public class SubjectAsync<T>
{
    private ISubjectAsync<T> MSubject { get; set; }
    
    public void SetSubject(ISubjectAsync<T> subject)
    {
        Debug.Log($"Subject received : {subject}");

        MSubject = subject; 
    }

    public ISubjectAsync<T> GetSubject()
    {
        return MSubject;
    }

    public async Task NotifySubject(T value, SemaphoreSlim lockingThread = null)
    {
       Debug.Log($"Here inside Subject Notify {value} {lockingThread} {MSubject}");

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
    public ISubjectAsync GetSubject()
    {
        return MSubject;
    }
    public async Task NotifySubject(SemaphoreSlim lockingThread = null)
    {
        await MSubject.OnNotifySubject(lockingThread);
    }
}
