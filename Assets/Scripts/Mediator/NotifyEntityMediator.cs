using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class NotifyEntityMediator: MonoBehaviour
{
    private List<NotifyPackage> NotifyEntities { get; set; } = new List<NotifyPackage>();
    private List<INotificationManager> NotificationManagers { get; set; } = new List<INotificationManager> { };

    private UnityEvent<NotifyPackage> m_notifyEntityEvent = new UnityEvent<NotifyPackage>();

    //get the list of Managers
    private async void Awake()
    {
        await AddListener(AppendNotifyEntity);
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

    private  GameObject[] GetAllNotificationManagers()
    {
        return (GameObject[])FindObjectsByType(typeof(INotificationManager), FindObjectsSortMode.None);
    }
}