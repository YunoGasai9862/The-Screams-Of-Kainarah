using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
public class NotifyEntityMediator : EntityPreloadMonoBehavior, IMediator
{
    private List<NotificationManagerPackage> NotificationManagerPackages { get; set; }
    private Dictionary<GameObject, INotificationManager> NotificationManagers { get; set; } = new Dictionary<GameObject, INotificationManager>();

    private Dictionary<INotificationManager, Type> NotificationManagersAndNotifierTypes { get; set; } = new Dictionary<INotificationManager, Type>();

    private MonoBehaviour[] MonoBehaviors { get; set; }

    private ScriptableObject[] ScriptableObjects { get; set; }
    private List<IMediatorNotificationListener> NotificationListeners { get; set; }

    private List<EntityPool<ScriptableObject>> PreloadedScriptableObjects { get; set; }

    private async void Start()
    {
        //maybe use a coroutine to halt the thread until this gets completed - we dont want race conditions

        await PrefillLookupDictionaries();

        await PingListeners(MonoBehaviors, ScriptableObjects);
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

    private async Task<List<NotificationManagerPackage>> GetNotificationPackages(MonoBehaviour[] monobehaviors)
    {
        Dictionary<GameObject, INotificationManager> NotificationManagers = monobehaviors.Select(mb => mb.gameObject).Where(mb => mb.GetComponent<INotificationManager>() != null).ToDictionary(

                gameObject => gameObject,
                gameObject => gameObject.GetComponent<INotificationManager>()
        );

        List<NotificationManagerPackage> notificationManagerPackages = await CreateNotificationManagerPackages(NotificationManagers);

        return notificationManagerPackages;
    }

    private Task<MonoBehaviour[]> QueryMonobehaviors()
    {
        return Task.FromResult((MonoBehaviour[])FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None));
    }

    private Task<ScriptableObject[]> QueryScriptableObjects()
    {
        //load all scriptable objects as assets and then query. THis can only be used if the object is in scene!!
        //just preload them all, and from EntityPool, Query them!!

        return null;
    }
    private Task<List<EntityPool<ScriptableObject>>> GetPreloadedScriptableObjects()
    {
        return null;
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
        MonoBehaviors = await QueryMonobehaviors();

        ScriptableObjects = await QueryScriptableObjects();

        foreach(var so  in ScriptableObjects)
        {
            Debug.Log($"Scriptable Objects {so.ToString()}");
        }

        NotificationManagerPackages = await GetNotificationPackages(MonoBehaviors);

        NotificationManagersAndNotifierTypes = await GenerateINotificationManagerAndNotifierTypeMap(NotificationManagerPackages);

    }

    private Task<List<IMediatorNotificationListener>> GetIMediatorNotificationListenersFromMonoBehaviors(MonoBehaviour[] monobehaviors)
    {
        return Task.FromResult(monobehaviors.Where(mb => mb.GetComponent<IMediatorNotificationListener>() != null).Select(mb => mb.GetComponent<IMediatorNotificationListener>()).ToList());
    }

    private Task<List<IMediatorNotificationListener>> GetIMediatorNotificationListenersFromScriptableObjects(ScriptableObject[] scriptableObjects)
    {
        var test = scriptableObjects.Where(so => so is IMediatorNotificationListener).Cast<IMediatorNotificationListener>().ToList();
        Debug.Log($"{test.Count}");
        return Task.FromResult(scriptableObjects.Where(so => so is IMediatorNotificationListener).Select(so => so).Cast<IMediatorNotificationListener>().ToList());
    }


    private async Task PingListeners(MonoBehaviour[] monobehaviors, ScriptableObject[] scriptableObjects)
    {
        Debug.Log("Inside Ping Listeners");

        NotificationListeners = await GetIMediatorNotificationListenersFromMonoBehaviors(monobehaviors);

        NotificationListeners.AddRange(await GetIMediatorNotificationListenersFromScriptableObjects(scriptableObjects)); ;

        Debug.Log($"Count of Listeners: {NotificationListeners.Count}");

        await Task.WhenAll(NotificationListeners.Select(listener => listener.MediatorNotificationListener()));
    }

    public override async Task<Tuple<EntityType, dynamic>> EntityPreload(dynamic assetReference, Asset entityType, Preloader preloader)
    {
        // GameObject mediatorPreloadInstance = (GameObject) await preloader.PreloadAsset<GameObject>(assetReference, entityType);

        //return new Tuple<EntityType, dynamic>(EntityType.MonoBehavior, mediatorPreloadInstance);

        return null;
    }
}