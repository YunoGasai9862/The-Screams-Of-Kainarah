using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour, IObserver<EntityPoolManager>
{
    public void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}