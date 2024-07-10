using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EntityListenerDelegator : MonoBehaviour 
{
    public async Task<bool> ListenerDelegator<T>(SubjectsToBeNotified<T> subjectsToNofity, T dataType, SemaphoreSlim lockingThread=null)
    {
        subjectsToNofity.NotifyObservers(dataType, lockingThread);

        return await Task.FromResult(true);
    }

}
