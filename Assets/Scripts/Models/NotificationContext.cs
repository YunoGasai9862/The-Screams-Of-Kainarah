using UnityEngine;

public class NotificationContext
{
    public string ObserverName { get; set; }
    public string ObserverTag { get; set; }
    public string SubjectType { get; set; }

    public override string ToString()
    {
        return $"ObserverName: {ObserverName} ObserverTag: {ObserverTag} SubjectType: {SubjectType}";
    }
}

public class NotificationContext<T> : NotificationContext
{
    public T ContextData { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, Context Data: {ContextData}";
    }
}