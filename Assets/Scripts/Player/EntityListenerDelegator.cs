using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EntityListenerDelegator : MonoBehaviour 
{
    public async Task<bool> ListenerDelegator<T>(ObserverList<T> subjectsToNofity, T dataType, NotificationContext notificationContext = null, SemaphoreSlim lockingThread=null)
    {
        subjectsToNofity.NotifyObservers(dataType, notificationContext, lockingThread);

        return await Task.FromResult(true);
    }

}
