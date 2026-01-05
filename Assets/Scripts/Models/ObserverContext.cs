using System;
using UnityEngine;

public class ObserverContext: Context
{
    public string SubjectType { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()} Subject Type: {SubjectType}";
    }
}

public class ObserverContext<T> : ObserverContext
{
    /// <summary>
    /// Context data to be passed along with the subject/observer type
    /// </summary>
    public T Data { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, Context Data: {Data}";
    }
}