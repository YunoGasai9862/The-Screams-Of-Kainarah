using UnityEngine;

public class NotificationContext
{
    public string GameObjectName { get; set; }
    public string GameObjectTag { get; set; }

    public override string ToString()
    {
        return $"GameObjectName: {GameObjectName} Tag: {GameObjectTag}";
    }
}