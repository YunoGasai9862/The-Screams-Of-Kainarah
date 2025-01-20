using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SceneSingletonDelegator : BaseDelegator<SceneSingleton>
{

    private SubjectAsync<IObserverAsync<SceneSingleton>> m_subject = new SubjectAsync<IObserverAsync<SceneSingleton>>();
    public override SubjectAsync<IObserverAsync<SceneSingleton>> Subject { get => m_subject; }

    private void Start()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;  
    }
}