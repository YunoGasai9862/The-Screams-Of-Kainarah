using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class NotifyEntityMediator: MonoBehaviour
{
    private List<NotificationManagerPackage> NotificationManagerPackages { get; set; }
    private Dictionary<GameObject, INotificationManager> NotificationManagers { get; set; } = new Dictionary<GameObject, INotificationManager>();

    private Dictionary<INotificationManager, Type> NotificationManagersAndNotifierTypes { get; set; } = new Dictionary<INotificationManager, Type>();
 
    private UnityEvent<NotifyPackage> m_notifyEntityEvent = new UnityEvent<NotifyPackage>();

    private async void Awake()
    {
        await AddListener(NotifiyManager);

        //maybe use a coroutine to halt the thread until this gets completed - we dont want race conditions
        await PrefillLookupDictionaries();
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

    private void NotifiyManager(NotifyPackage notifyPackage)
    {
        //continue here for calling manager
    }

    private async Task<List<NotificationManagerPackage>> GetNotificationPackages()
    {
        MonoBehaviour[] monoBehaviorObjects = (MonoBehaviour[])FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None);

        Dictionary<GameObject, INotificationManager> NotificationManagers = monoBehaviorObjects.Select(mb => mb.gameObject).Where(mb => mb.GetComponent<INotificationManager>() != null).ToDictionary(

                gameObject => gameObject,
                gameObject => gameObject.GetComponent<INotificationManager>()
        );

        List<NotificationManagerPackage> notificationManagerPackages = await CreateNotificationManagerPackages(NotificationManagers);

        return notificationManagerPackages;
    }

    private Task InvokeCustomMethods(List<INotificationManager> notificationManagers)
    {
        return Task.CompletedTask;
    }

    private Task<List<NotificationManagerPackage>> CreateNotificationManagerPackages(Dictionary<GameObject, INotificationManager> notificationManagerObjects)
    {
        List<NotificationManagerPackage> notificationManagerPackages = new List<NotificationManagerPackage>();

        foreach(KeyValuePair<GameObject, INotificationManager> notificationManager in notificationManagerObjects)
        {

            notificationManagerPackages.Add(new NotificationManagerPackage { INotificationManager = notificationManager.Value, NotificationManagerObject = notificationManager.Key });
        }

        return Task.FromResult(notificationManagerPackages);
    }

    private Task<Dictionary<INotificationManager, Type>> GenerateINotificationManagerAndNotifierTypeMap(List<NotificationManagerPackage> notificationManagerPackages)
    {
        Dictionary<INotificationManager, Type> notifcationManagerAndNotifierDict = new Dictionary<INotificationManager, Type>();

        foreach(NotificationManagerPackage notificationManagerPackage in notificationManagerPackages)
        {
            notifcationManagerAndNotifierDict[notificationManagerPackage.INotificationManager] = notificationManagerPackage.GetType();
        }

        return Task.FromResult(notifcationManagerAndNotifierDict);
    }

    private async Task PrefillLookupDictionaries()
    {
        NotificationManagerPackages = await GetNotificationPackages();

        NotificationManagersAndNotifierTypes = await GenerateINotificationManagerAndNotifierTypeMap(NotificationManagerPackages);
    }
}