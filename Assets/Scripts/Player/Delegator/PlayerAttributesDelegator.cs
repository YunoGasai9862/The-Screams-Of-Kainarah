using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerAttributesDelegator : BaseDelegatorEnhanced<Transform>, IObserver<ObserverSystemAttributeHelper>
{
    [SerializeField]
    ObserverSystemAttributeDelegator observerSystemAttributeDelegator;
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<Transform>>>();
    }

    private void Start()
    {
        StartCoroutine(observerSystemAttributeDelegator.NotifySubject(this));
    }

    public void OnNotify(ObserverSystemAttributeHelper data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        ObserverSubjectDict = data.GetObserverSubjectDict();
    }
}