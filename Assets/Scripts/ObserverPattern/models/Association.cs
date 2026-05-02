using ObserverPattern;
using UnityEngine;

public class Association<T>
{
    public IObserver<T> Observer { get; set; }
    
    public Subject<T> Subject { get; set; }

    public override string ToString()
    {
        return $"Observer: {Observer}, Subject : {Subject}";
    }
}
public class Association
{
    public GameObject ObserverInstance { get; set; }

    public GameObject SubjectInstance { get; set; }

    public override string ToString()
    {
        return $"Observer: {ObserverInstance}, Subject : {SubjectInstance}";
    }
}