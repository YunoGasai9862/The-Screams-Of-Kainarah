using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SubjectsToBeNotified : MonoBehaviour
{

    private List<IObserver<Collider2D>> _potentialObservers = new();
    private List<IObserver<bool>> _potentialBoolObservers = new();

    public void AddObserver(IObserver<Collider2D> observer)
    {
        _potentialObservers.Add(observer);
    }

    public void RemoveOberver(IObserver<Collider2D> observer)
    {
        _potentialObservers.Remove(observer);
    }

    public void AddObserver(IObserver<bool> observer)
    {
        _potentialBoolObservers.Add(observer);
    }

    public void RemoveOberver(IObserver<bool> observer)
    {
        _potentialBoolObservers.Remove(observer);
    }

    protected void NotifyObservers(ref Collider2D collider)
    {
        foreach (var observer in _potentialObservers)
        {
            observer.OnNotify(ref collider);
        }
    }

    protected void NotifyObservers(bool value)
    {
        foreach (var observer in _potentialBoolObservers)
        {
            observer.OnNotify(ref value);
        }
    }


}
