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