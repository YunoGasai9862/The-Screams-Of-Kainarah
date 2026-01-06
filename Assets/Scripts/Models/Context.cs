using System;
using UnityEngine;

public class Context
{
    public string Name { get; set; }
    public string Tag { get; set; }

    public Type EntityType { get; set; }
    public override string ToString()
    {
        return $"Name: {Name} Tag: {Tag} EntityType: {EntityType}";
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