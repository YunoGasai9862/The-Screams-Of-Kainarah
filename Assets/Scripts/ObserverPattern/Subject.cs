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

    public async Task NotifySubject(T value, SemaphoreSlim lockingThread = null)
    {
       await MSubject.OnNotifySubject(value, lockingThread);
    }
}

/// <summary>
/// Represents a subject for synchronous observer pattern
/// <typeparam name="T">The type T here is the observer's interface type that the subject notifies</typeparam>
/// </summary>
public class Subject<T>
{
    private ISubject<T> MSubject { get; set; }

    public void SetSubject(ISubject<T> subject)
    {
        MSubject = subject;
    }

    public ISubject<T> GetSubject()
    {
        return MSubject;
    }

    public void NotifySubject(T value, NotificationContext notificationContext, SemaphoreSlim lockingThread = null)
    {
        MSubject.OnNotifySubject(value, notificationContext, lockingThread);
    }
}

public class Subjects<T>
{
    private Dictionary<string, ISubject<T>> SubjectsDict { get; set; } = new Dictionary<string, ISubject<T>>();

    public Task SetSubject(string key, ISubject<T> subject)
    {
        SubjectsDict.Add(key, subject);

        return Task.CompletedTask;
    }

    public Task<ISubject<T>> GetSubject(string key)
    {
        if (SubjectsDict.TryGetValue(key, out ISubject<T> subject))
        {
            return Task.FromResult(subject);
        }

        return null;
    }

    public void NotifySubjects(T value, NotificationContext notificationContext, SemaphoreSlim lockingThread = null)
    {
        foreach(ISubject<T> subject in SubjectsDict.Values)
        {
            subject.OnNotifySubject(value, notificationContext, lockingThread);
        }
    }

    public void NotifySubject(string key, T value, NotificationContext notificationContext, SemaphoreSlim lockingThread = null)
    {
        if (SubjectsDict.TryGetValue(key, out ISubject<T> subject))
        {
            subject.OnNotifySubject(value, notificationContext, lockingThread);
        }
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
