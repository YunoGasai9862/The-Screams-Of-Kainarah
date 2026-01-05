using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


/// <summary>
/// Represents a base/barebones subject withg type only for the observer pattern
/// </summary>

public class BaseSubject
{
    public Type SubjectType { get; set; }

    public BaseSubject(Type subjectType)
    {
        SubjectType = subjectType;
    }
}

/// <summary>
/// Represents a subject for asynchronous observer pattern
/// <typeparam name="T">The type T here is the observer's interface type that the subject notifies</typeparam>
/// </summary>
public class SubjectAsync<T>: BaseSubject
{
    public ISubjectAsync<T> MSubject { get; set; }

    public SubjectAsync(ISubjectAsync<T> subject, Type type): base(type)
    {
        MSubject = subject;
    }
    
    public async Task NotifySubject(IObserver<T> value, SemaphoreSlim lockingThread = null, params object[] optional)
    {
       await MSubject.OnNotifySubject(value, lockingThread, optional);
    }
}

/// <summary>
/// Represents a subject for synchronous observer pattern
/// <typeparam name="T">The type T here is the observer's interface type that the subject notifies</typeparam>
/// </summary>
public class Subject<T> : BaseSubject
{
    public ISubject<T> ISubject { get; set; }

    public Subject(ISubject<T> subject, Type type) : base(type)
    {
        ISubject = subject;
    }

    public void NotifySubject(IObserver<T> value, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim lockingThread = null, params object[] optional)
    {
        ISubject.OnNotifySubject(value, context, cancellationToken, lockingThread, optional);
    }

}
public class SubjectAsync: BaseSubject
{
    public ISubjectAsync MSubject { get; set; }

    public SubjectAsync(ISubjectAsync subject, Type type) : base(type)
    {
        this.MSubject = subject;
    }

    public async Task NotifySubject(SemaphoreSlim lockingThread = null)
    {
        await MSubject.OnNotifySubject(lockingThread);
    }
}
