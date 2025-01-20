using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegatorAsync<T>
{
    protected CancellationToken CancellationToken { get; set; }
    protected CancellationTokenSource CancellationTokenSource { get; set; }

    public abstract SubjectAsync<IObserverAsync<T>> Subject { get; }

    public async Task NotifyObserver(IObserverAsync<T> observer, T value)
    {
        await observer.OnNotify(value, CancellationToken);
    }

    public async Task NotifySubject(IObserverAsync<T> observer)
    {
        await Subject.NotifySubject(observer);
    }
}