using UnityEngine;
using UnityEngine.UIElements;

public class NotificationManagerPackage
{
    public GameObject NotificationManagerObject { get; set; }
    public INotificationManager INotificationManager { get; set; }

    public override string ToString()
    {
        return $"Object: {NotificationManagerObject}, NotificationManager: {INotificationManager}";
    }
}