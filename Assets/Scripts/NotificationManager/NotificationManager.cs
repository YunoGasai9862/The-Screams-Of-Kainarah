//create a notification manager system, where it will keep a custom class list
//and track the status of initalized
//when an event is received from a class that requires to be present
//check if the entire list state is set to true
//if so send notification to preload manager, that it can start with prelaoding
//to avoid null exceptions
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class NotificationManager: MonoBehaviour, INotifcation
{
    [SerializeField]
    public List<NotifyEntity> notifyEntities;

    [SerializeField]
    public List<Listener> notifyingEntities;

    private List<IListenerEntity> ListenerEntities { get; set;}

    private int ListenerEntityCount { get; set; }

    private async void Start()
    {
        ListenerEntities = await FilterOutListenerEntities(notifyingEntities);
    }

    private void Update()
    {
        // mechanism to stop as well if all of those are true
    }

    public Task NotifyEntity(List<IListenerEntity> notifyingEntities)
    {
       IEnumerable<Task> tasks = notifyingEntities.Select(notifyingEntity => notifyingEntity.Listen());

        return Task.WhenAll(tasks);
    }

    public Task UpdateNotifyList(string tag, bool isActive)
    {
        NotifyEntity notifyEntity = notifyEntities.Where(notifyEntity => notifyEntity.Tag == tag).FirstOrDefault();

        if (notifyEntity == null)
        {
            throw new System.Exception($"Notify Entity with {tag} doesn't exist!");
        }

        notifyEntity.IsActive = isActive;

        return Task.CompletedTask;
    }

    private Task<List<IListenerEntity>> FilterOutListenerEntities(List<Listener> notifyingEntities)
    {
        return Task.FromResult(notifyingEntities.Select(notifyingEntity => notifyingEntity.gameObject.GetComponent<IListenerEntity>()).Where(component => component != null).ToList());
    }
}