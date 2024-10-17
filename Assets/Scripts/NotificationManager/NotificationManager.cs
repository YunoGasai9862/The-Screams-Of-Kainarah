//create a notification manager system, where it will keep a custom class list
//and track the status of initalized
//when an event is received from a class that requires to be present
//check if the entire list state is set to true
//if so send notification to preload manager, that it can start with prelaoding
//to avoid null exceptions
using System.Threading.Tasks;
using UnityEngine;
public class NotificationManager: MonoBehaviour, INotifcation
{
    public Task NotifyEntity()
    {
        return Task.CompletedTask;
    }

    public Task UpdateNotifyList()
    {
        return Task.CompletedTask;
    }
}