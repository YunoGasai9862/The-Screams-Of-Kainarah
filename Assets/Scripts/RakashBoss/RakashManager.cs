using System;
using System.Threading;
using UnityEngine;
using static SceneData;
public class RakashManager : AbstractEntity, IGameStateHandler, ISubject<IObserver<PlayerVariables>>
{
    [SerializeField]
    HealthDelegator healthDelegator;
    [SerializeField]
    HealthEvent healthEvent;

    public override PlayerVariables Health {

        get {

            if (Health == null)
            {
                Health = new PlayerVariables()
                {
                    MaxHealth = 100f,
                    CurrentHealth = 100f,
                    EntityName = gameObject.name
                };
            }

            return Health;
        
        } 
        
        set => Health = value;
    }

    void Start()
    {
        healthDelegator.AddToSubjectsDict(typeof(RakashManager).ToString(), name, new Subject<IObserver<PlayerVariables>>());

        healthDelegator.GetSubsetSubjectsDictionary(typeof(RakashManager).ToString())[name].SetSubject(this);

        healthEvent.AddListener(UpdateHealth);

        SceneSingleton.InsertIntoGameStateHandlerList(this);
    }

    public override void GameStateHandler(SceneData data)
    {
        data.AddToObjectsToPersist(new ObjectData(transform.tag, transform.name, transform.position, transform.rotation));
    }

    public void OnNotifySubject(IObserver<PlayerVariables> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(healthDelegator.NotifyObserver(data, Health, new NotificationContext()
        {
            SubjectType = typeof(RakashManager).ToString()  
        }, CancellationToken.None));
    }

    private void UpdateHealth(float newHealth)
    {
        Health.CurrentHealth = newHealth;
    }
}
