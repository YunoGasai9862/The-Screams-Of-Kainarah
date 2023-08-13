using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightObserverPattern : MonoBehaviour
{
    private List<IObserverAsync<Candle>> subjectsToBeadded = new();
    public void AddObserver(IObserverAsync<Candle> subject)
    {
        subjectsToBeadded.Add(subject);
    }

    public void RemoveObserver(IObserverAsync<Candle> subject)
    {
        subjectsToBeadded.Remove(subject);
    }

    public async Task NotifyAllLightObserversAsync(Candle _candleProperties, CancellationToken _cancellationToken)
    {
        foreach (IObserverAsync<Candle> subject in subjectsToBeadded)
        {
            await subject.OnNotify(_candleProperties, _cancellationToken);

            if (_cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Cancelling");
                throw new OperationCanceledException(_cancellationToken);
            }
        }
    }
}
