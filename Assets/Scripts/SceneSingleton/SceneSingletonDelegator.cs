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

    private SubjectAsync m_subject = new SubjectAsync();
    public SubjectAsync Subject { get => m_subject; }

    private void Start()
    {
        //initialize tokens!
    }

    public async Task NotifyObserver(IObserverAsync<SceneSingleton> observer, SceneSingleton value)
    {
        await observer.OnNotify(value, CancellationToken);
    }

    public async Task NotifySubject()
    {
        await Subject.NotifySubject();
    }

    public Task NotifySubject(IObserverAsync<SceneSingleton> observer)
    {
        throw new NotImplementedException();
    }
}