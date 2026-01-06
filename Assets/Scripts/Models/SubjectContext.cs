using System;
using UnityEngine;

public class SubjectContext: Context
{
}

public class SubjectContext<T> : SubjectContext
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