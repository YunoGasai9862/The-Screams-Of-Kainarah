//create a notification manager system, where it will keep a custom class list
//and track the status of initalized
//when an event is received from a class that requires to be present
//check if the entire list state is set to true
//if so send notification to preload manager, that it can start with prelaoding
//to avoid null exceptions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class NotificationManager: NotifierEntityDelegate, INotification, INotificationManager
{
    [SerializeField]
    public List<NotifierEntity> notifyEntities;

    [SerializeField]
    public List<Listener> notifyingEntities;

    private List<IListenerEntity> ListenerEntities { get; set;}

    public override IGenericDelegate<NotifierEntity>.InvokeMethod InvokeCustomMethod { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private void Start()
    {
        InvokeCustomMethod += InvokePingNotificationManagerMethod;
    }

    public Task NotifyEntity(List<IListenerEntity> notifyingEntities)
    {
        IEnumerable<Task> tasks = notifyingEntities.Select(notifyingEntity => notifyingEntity.Listen());

        return Task.WhenAll(tasks);
    }

    public async Task PingNotificationManager(NotifierEntity notifyEntity)
    {
        NotifierEntity entity = notifyEntities.Where(ne => ne.Tag == notifyEntity.Tag).FirstOrDefault();

        if (entity == null)
        {
            throw new System.Exception($"Notify Entity with {entity.Tag} doesn't exist!");
        }

        entity.IsActive = notifyEntity.IsActive;

        await CheckIfAllEntitiesAreActive(notifyEntities);
    }

    private Task<List<IListenerEntity>> FilterOutListenerEntities(List<Listener> notifyingEntities)
    {
        return Task.FromResult(notifyingEntities.Select(notifyingEntity => notifyingEntity.gameObject.GetComponent<IListenerEntity>()).Where(component => component != null).ToList());
    }

    private async Task CheckIfAllEntitiesAreActive(List<NotifierEntity> entities)
    {
        if (entities.All(entity => entity.IsActive))
        {
            ListenerEntities = await FilterOutListenerEntities(notifyingEntities);

            await NotifyEntity(ListenerEntities);
        }
    }

    private async void InvokePingNotificationManagerMethod(NotifierEntity notifyEntity)
    {
        Debug.Log("Getting Notified");

        await PingNotificationManager(notifyEntity);
    }
}