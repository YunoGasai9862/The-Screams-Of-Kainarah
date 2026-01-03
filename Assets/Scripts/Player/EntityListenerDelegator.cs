using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EntityListenerDelegator : MonoBehaviour 
{
    public async Task<bool> ListenerDelegator<T>(ObserverList<T> subjectsToNofity, T dataType, Context context = null, SemaphoreSlim lockingThread=null)
    {
        subjectsToNofity.NotifyObservers(dataType, CancellationToken.None, context, lockingThread);

        return await Task.FromResult(true);
    }

}
