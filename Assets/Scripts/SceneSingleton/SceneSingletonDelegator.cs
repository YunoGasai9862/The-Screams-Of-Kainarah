using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SceneSingletonDelegator : MonoBehaviour, IDelegatorAsync<SceneSingleton>
{
    //this is better, no queue etc! Now simply, use a coroutine in the observer calss to bloack the thread, or
    //keep pinging the delegator until u get a response back and then unblock the thread and continue
    //to avoid race conditions
    //queue shouldn't be use because each entity can come alive anytime
    private CancellationToken CancellationToken { get; set; }
    private CancellationTokenSource CancellationTokenSource { get; set; }

    private SubjectAsync<IObserverAsync<SceneSingleton>> m_subject = new SubjectAsync<IObserverAsync<SceneSingleton>>();
    public SubjectAsync<IObserverAsync<SceneSingleton>> Subject { get => m_subject; }

    private void Start()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;  
    }

    public async Task NotifyObserver(IObserverAsync<SceneSingleton> observer, SceneSingleton value)
    {
        await observer.OnNotify(value, CancellationToken);
    }

    public async Task NotifySubject(IObserverAsync<SceneSingleton> observer)
    {
        await Subject.NotifySubject(observer);
    }
}