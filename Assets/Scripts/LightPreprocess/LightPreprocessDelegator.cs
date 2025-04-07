using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegatorEnhanced<ILightPreprocess>, IObserver<ObserverSystemAttributeHelper>
{
    [SerializeField]
    public ObserverSystemAttributeDelegator observerSystemAttributeDelegator;

    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<ILightPreprocess>>>();
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