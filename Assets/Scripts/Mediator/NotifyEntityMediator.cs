using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
public class NotifyEntityMediator: EntityPreloadMonoBehavior, IMediator
{
    private List<NotificationManagerPackage> NotificationManagerPackages { get; set; }
    private Dictionary<GameObject, INotificationManager> NotificationManagers { get; set; } = new Dictionary<GameObject, INotificationManager>();

    private Dictionary<INotificationManager, Type> NotificationManagersAndNotifierTypes { get; set; } = new Dictionary<INotificationManager, Type>();
 
    private async void Start()
    {
        //maybe use a coroutine to halt the thread until this gets completed - we dont want race conditions
        await PrefillLookupDictionaries();
    }

    public async Task NotifyManager(NotifyPackage notifyPackage)
    {
        Debug.Log($"{notifyPackage.ToString()}");

        INotificationManager notificationManager = NotificationManagers.Where(keyValuePair => keyValuePair.Key.tag == notifyPackage.EntityNameToNotify).Select(keyValuePair => keyValuePair.Value).SingleOrDefault();

        if (notificationManager == null)
        {
            throw new Exception($"Notification Script is Null for: {notifyPackage.EntityNameToNotify}");
        }

        if (NotificationManagersAndNotifierTypes.TryGetValue(notificationManager, out Type notificationType)) {

            notifyPackage = await CastTo(notificationType, notifyPackage);
            Debug.Log(notifyPackage);
        }

    }

    private Task<NotifyPackage> CastTo(Type notifyPackageType, NotifyPackage notifyPackage)
    {
        switch (notifyPackageType)
        {
            case Type _ when notifyPackageType == typeof(NotifierEntity):
                notifyPackage.NotifierEntity = (NotifierEntity)notifyPackage.NotifierEntity;
                break;

            default:
                break;
        }

        return Task.FromResult(notifyPackage);
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

    private Task PingListeners()
    {
        return Task.CompletedTask;
    }

}