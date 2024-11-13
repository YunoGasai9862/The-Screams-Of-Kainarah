using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class NotifyEntityMediator: MonoBehaviour
{
    private List<NotifyPackage> NotifyEntities { get; set; } = new List<NotifyPackage>();
    private List<INotificationManager> NotificationManagers { get; set; }
    private List<NotificationManagerPackage> NotificationManagerPackages { get; set; }

    private UnityEvent<NotifyPackage> m_notifyEntityEvent = new UnityEvent<NotifyPackage>();

    private async void Awake()
    {
        await AddListener(AppendNotifyEntity);
    }

    private async void Start()
    {
        (NotificationManagers, NotificationManagerPackages) = await GetAllNotificationManagersAndPackages();
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

    private async Task<(List<INotificationManager>, List<NotificationManagerPackage>)> GetAllNotificationManagersAndPackages()
    {
        MonoBehaviour[] monoBehaviorObjects = (MonoBehaviour[])FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None);

        List<GameObject> notificationManagerObject = monoBehaviorObjects.Select(mb => mb.gameObject).Where(mb => mb.GetComponent<INotificationManager>() != null).ToList();

        List<INotificationManager> notificationManagers = notificationManagerObject.Select(mb => mb.GetComponent<INotificationManager>()).ToList();

        List<NotificationManagerPackage> notificationManagerPackages = await CreateNotificationManagerPackages(notificationManagerObject);

        return (notificationManagers, notificationManagerPackages);
    }

    private Task InvokeCustomMethods(List<INotificationManager> notificationManagers)
    {
        return Task.CompletedTask;
    }

    private Task<List<NotificationManagerPackage>> CreateNotificationManagerPackages(List<GameObject> notificationManagerObjects)
    {
        List<NotificationManagerPackage> notificationManagerPackages = new List<NotificationManagerPackage>();

        foreach(GameObject notificationManagerObject in notificationManagerObjects)
        {
           // notificationManagerPackages.Add(new NotificationManagerPackage { INotificationManager = notificationManager, NotificationManagerObject = notificationManager });
        }

        return Task.FromResult(notificationManagerPackages);
    }
}