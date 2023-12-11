using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerObserverListener : MonoBehaviour //try something new (delegates)
{
    public async Task<bool> ListenerDelegator<T>(SubjectsToBeNotified<T> subjectsToNofity,  T dataType)
    {
        subjectsToNofity.NotifyObservers(dataType);
        return await Task.FromResult(true);
    }

}
