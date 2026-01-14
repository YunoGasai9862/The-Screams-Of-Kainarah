using System;
using UnityEngine;

public class ObserverContext<T>: Context
{
    public INotify<T> INotify { get; set; }
    public Type SubjectType { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()} Subject Type: {SubjectType}, INotify<{typeof(T).Name}>";
    }
}
