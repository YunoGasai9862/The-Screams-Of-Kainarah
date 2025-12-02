
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ResetAttribute : Attribute
{
    public Type Type { get; set; }

    public ResetAttribute(Type type)
    {
        this.Type = type;
    }
}