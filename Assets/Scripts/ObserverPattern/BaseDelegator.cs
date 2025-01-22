using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseDelegator<T> : MonoBehaviour, IDelegator<T>
{
    public Subject<IObserver<T>> Subject { get; set; }

    public IEnumerator NotifyObserver(IObserver<T> observer, T value)
    {
        observer.OnNotify(value);

        yield return null;
    }

    public IEnumerator NotifySubject(IObserver<T> observer)
    {
        Debug.Log($"Notifying Subject: {Subject} from observer {observer} Main Subject {Subject.GetSubject()}");

        StartCoroutine(Helper.WaitUntilVariableIsNonNull(Subject.GetSubject()));

        Subject.NotifySubject(observer);

        yield return null;
    }
}