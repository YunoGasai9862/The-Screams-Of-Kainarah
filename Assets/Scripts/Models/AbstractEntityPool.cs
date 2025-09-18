using System;
using UnityEngine;

public abstract class AbstractEntityPool<T>
{
    public string Name { get; set; }
    public string Tag { get; set; }
    public T  Entity { get; set; }
    public override string ToString()
    {
        return $"Name: {Name}, Tag: {Tag}, Object: {Entity}";
    }
}