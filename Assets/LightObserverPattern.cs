using System.Collections.Generic;
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

    public async Task NotifyAllLightObserversAsync(Candle _candleProperties)
    {
        foreach (IObserverAsync<Candle> subject in subjectsToBeadded)
        {
            await subject.OnNotify(_candleProperties);
        }
    }
}
