using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightObserverPattern : MonoBehaviour
{
    private List<IObserverAsync<LightEntity>> subjectsToBeadded = new();
    public void AddObserver(IObserverAsync<LightEntity> subject)
    {
        subjectsToBeadded.Add(subject);
    }

    public void RemoveObserver(IObserverAsync<LightEntity> subject)
    {
        subjectsToBeadded.Remove(subject);
    }

    public async Task NotifyAllLightObserversAsync(LightEntity lightProperties, CancellationToken _cancellationToken, SemaphoreSlim semaphore = null)
    {
        foreach (IObserverAsync<LightEntity> subject in subjectsToBeadded)
        {
            Debug.Log("Notifying for flicker!");
            await subject.OnNotify(lightProperties, _cancellationToken);

            if (semaphore != null)
                semaphore.Release();

            if (_cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Cancelling");
                throw new OperationCanceledException(_cancellationToken);
            }
        }
    }
}
