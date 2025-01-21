using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SceneSingletonDelegator : BaseDelegator<SceneSingleton>
{
    private void OnEnable()
    {
        Subject = new SubjectAsync<IObserverAsync<SceneSingleton>>();

        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }
}