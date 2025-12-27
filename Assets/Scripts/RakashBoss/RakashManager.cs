using System;
using System.Threading;
using UnityEngine;
using static SceneData;
public class RakashManager : AbstractEntity, IGameStateHandler, ISubject<Health>
{
    private HealthDelegator HealthDelegator { get; set; }

    public override Health Health { get; set; }

    private void Awake()
    {
        Health = new Health()
        {
            MaxHealth = 100f,
            CurrentHealth = 100f,
            EntityName = gameObject.name
        };
    }

    private async void Start()
    {
        HealthDelegator = await Helper.GetDelegator<HealthDelegator>();

        HealthDelegator.AddToSubjectsDict(typeof(RakashManager).ToString(), name, new Subject<Health>(this, typeof(RakashManager)));

        SceneSingleton.InsertIntoGameStateHandlerList(this);
    }

    public override void GameStateHandler(SceneData data)
    {
        data.AddToObjectsToPersist(new ObjectData(transform.tag, transform.name, transform.position, transform.rotation));
    }

    public void OnNotifySubject(IObserver<Health> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(HealthDelegator.NotifyObserver(data, Health, new NotificationContext()
        {
            SubjectType = typeof(RakashManager).ToString()  
        }, CancellationToken.None));
    }
}
