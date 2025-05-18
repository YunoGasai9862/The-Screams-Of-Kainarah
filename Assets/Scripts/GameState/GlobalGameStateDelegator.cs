using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GlobalGameStateDelegator: BaseDelegatorEnhanced<GameState>, IObserver<ObserverSystemAttributeHelper>
{

    [SerializeField]
    ObserverSystemAttributeDelegator observerSystemAttributeDelegator;

    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GameState>>>>();
    }

    private void Start()
    {
        StartCoroutine(observerSystemAttributeDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(ObserverSystemAttributeHelper).ToString()
        }, CancellationToken.None));
    }

    public void OnNotify(ObserverSystemAttributeHelper data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
       ObserverSubjectDict = data.GetObserverSubjectDict();
    }

}