using System;
using System.Collections.Generic;
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
        MSubject = subject; 
    }

    public ISubjectAsync<T> GetSubject()
    {
        return MSubject;
    }

    public async Task NotifySubject(T value, SemaphoreSlim lockingThread = null, params object[] optional)
    {
       await MSubject.OnNotifySubject(value, lockingThread, optional);
    }
}

/// <summary>
/// Represents a subject for synchronous observer pattern
/// <typeparam name="T">The type T here is the observer's interface type that the subject notifies</typeparam>
/// </summary>
public class Subject<T>
{
    private ISubject<T> ISubject { get; set; }

    public void SetSubject(ISubject<T> subject)
    {
        ISubject = subject;
    }

    public ISubject<T> GetSubject()
    {
        return ISubject;
    }

    public void NotifySubject(T value, NotificationContext notificationContext, SemaphoreSlim lockingThread = null, params object[] optional)
    {
        ISubject.OnNotifySubject(value, notificationContext, lockingThread, optional);
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
