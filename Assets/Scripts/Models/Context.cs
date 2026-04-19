#nullable enable
using System;
using UnityEngine;
public class Context
{
    public GameObject Instance { get; set; }
    public Type EntityType { get; set; }

    public FallBackAlert? FallBack { get; set; }

    public class FallBackAlert
    {
        public  Action Alert { get; set; }
    }

    public override string ToString()
    {
        return $"Instance: {Instance} EntityType: {EntityType}";
    }
}

public class Context<T> : Context
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