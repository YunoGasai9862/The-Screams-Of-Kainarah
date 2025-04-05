using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ObserverSystemAttribute: Attribute
{
    public Type SubjectType { get; set; }

    public Type ObserverType { get; set; }

    public ObserverSystemAttribute()
    {

    }

    public override string ToString()
    {
        return $"Subject Type: {SubjectType}, ObserverType: {ObserverType}"; 
    }
}
