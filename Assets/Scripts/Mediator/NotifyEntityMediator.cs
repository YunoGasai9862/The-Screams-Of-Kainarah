using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class NotifyEntityMediator: MonoBehaviour
{
    private List<NotifyPackage> NotifyEntities { get; set; } = new List<NotifyPackage>();
    private List<INotificationManager> NotificationManagers { get; set; }

    private UnityEvent<NotifyPackage> m_notifyEntityEvent = new UnityEvent<NotifyPackage>();

    //get the list of Managers
    private async void Awake()
    {
        await AddListener(AppendNotifyEntity);
    }

    private void Start()
    {
        NotificationManagers = GetAllNotificationManagers();
    }

    public UnityEvent<NotifyPackage> GetInstance()
    {
        return m_notifyEntityEvent;
    }
    public Task AddListener(UnityAction<NotifyPackage> action)
    {
        Debug.Log("Adding Listener");

        m_notifyEntityEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public Task Invoke(NotifyPackage value)
    {
        Debug.Log($"Invoking {value.ToString()}");

        m_notifyEntityEvent.Invoke(value);

        return Task.CompletedTask;
    }

    private void AppendNotifyEntity(NotifyPackage notifyPackage)
    {
        NotifyEntities.Add(notifyPackage);
    }

    private List<INotificationManager> GetAllNotificationManagers()
    {
        MonoBehaviour[] monoBehaviorObjects = (MonoBehaviour[])FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None);

        List<INotificationManager> notificationManagers = monoBehaviorObjects.Select(mb => mb.GetComponent<INotificationManager>()).Where(INotificationManager => INotificationManager != null).ToList();

        return notificationManagers;
    }

    private Task InvokeCustomMethods(List<INotificationManager> notificationManagers)
    {
        return Task.CompletedTask;
    }
}