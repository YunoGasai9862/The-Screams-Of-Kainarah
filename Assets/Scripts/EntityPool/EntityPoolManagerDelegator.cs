using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EntityPoolManagerDelegator : BaseDelegator<EntityPoolManager>  
{

    private SubjectAsync<IObserverAsync<EntityPoolManager>> m_subject = new SubjectAsync<IObserverAsync<EntityPoolManager>>();
    public override SubjectAsync<IObserverAsync<EntityPoolManager>> Subject { get => m_subject; }

    private void Start()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }
}