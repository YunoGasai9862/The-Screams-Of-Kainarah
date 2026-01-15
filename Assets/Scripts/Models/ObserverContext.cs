using System;
using UnityEngine;

public class ObserverContext: Context
{
    public Type SubjectType { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()} Subject Type: {SubjectType}";
    }
}
