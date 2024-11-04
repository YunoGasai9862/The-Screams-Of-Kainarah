using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class NotifyEntityMediator: MonoBehaviour
{
    private List<NotifyEntity> NotifyEntities { get; set; } = new List<NotifyEntity>();

    private UnityEvent<NotifyEntity> m_notifyEntityEvent = new UnityEvent<NotifyEntity>();

    private async void Awake()
    {
        await AddListener(AppendNotifyEntity);
    }
    public UnityEvent<NotifyEntity> GetInstance()
    {
        return m_notifyEntityEvent;
    }
    public Task AddListener(UnityAction<NotifyEntity> action)
    {
        Debug.Log("Adding Listener");

        m_notifyEntityEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public Task Invoke(NotifyEntity value)
    {
        Debug.Log($"Invoking {value.ToString()}");

        m_notifyEntityEvent.Invoke(value);

        return Task.CompletedTask;
    }

    private void AppendNotifyEntity(NotifyEntity notifyEntity)
    {
        NotifyEntities.Add(notifyEntity);
    }
}