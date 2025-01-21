using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EntityPoolManagerDelegator : BaseDelegator<EntityPoolManager>  
{
    private void OnEnable()
    {
        Subject = new SubjectAsync<IObserverAsync<EntityPoolManager>>();

        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }

    public override async Task NotifySubject(IObserverAsync<EntityPoolManager> observer)
    {
        StartCoroutine(Helper.WaitUntilVariableIsNonNull(Subject.GetSubject()));

        Debug.Log($"Final Subject: {Subject.GetSubject()}");

        await base.NotifySubject(observer);
    }

}