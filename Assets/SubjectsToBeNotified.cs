using System.Collections.Generic;
using UnityEngine;

public class SubjectsToBeNotified : MonoBehaviour
{

    private List<IObserver<> _potentialObservers = new ();

    public void AddObserver(IObserver observer)
    {
        _potentialObservers.Add(observer);
    }

    public void RemoveOberver(IObserver observer)
    {
        _potentialObservers.Remove(observer);
    }

    protected void NotifyObservers(Collider2D collider)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(ref collider);
        }
    }

    protected void NotifyObservers(bool value)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(value);
        }
    }


}
