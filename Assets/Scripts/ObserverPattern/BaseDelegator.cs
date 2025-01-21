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

    public SubjectAsync<IObserverAsync<T>> Subject { get; set; }

    public virtual async Task NotifyObserver(IObserverAsync<T> observer, T value)
    {
        Debug.Log($"Here in Notify Observer! {observer} {value}");

        await observer.OnNotify(value, CancellationToken);
    }

    public virtual async Task NotifySubject(IObserverAsync<T> observer)
    {
        Debug.Log($"Notifying Subject: {Subject} from observer {observer} Main Subject {Subject.GetSubject()}");

        await Subject.NotifySubject(observer);
    }
}