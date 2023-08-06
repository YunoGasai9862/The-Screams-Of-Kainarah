using System.Collections.Generic;
using UnityEngine;

public class LightObserverPattern : MonoBehaviour
{
    private List<IObserver<Candle>> subjectsToBeadded = new();
    public void AddObserver(IObserver<Candle> subject)
    {
        subjectsToBeadded.Add(subject);
    }

    public void RemoveObserver(IObserver<Candle> subject)
    {
        subjectsToBeadded.Remove(subject);
    }

    public void NotifyAllLightObservers(Candle _candleProperties)
    {
        foreach (IObserver<Candle> subject in subjectsToBeadded)
        {
            subject.OnNotify(ref _candleProperties);
        }
    }
}
