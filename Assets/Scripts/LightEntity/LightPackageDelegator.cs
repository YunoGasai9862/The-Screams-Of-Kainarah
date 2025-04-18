using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class LightPackageDelegator: BaseDelegatorEnhanced<LightPackage>, IObserver<ObserverSystemAttributeHelper>
{
    [SerializeField]
    public ObserverSystemAttributeDelegator observerSystemAttributeDelegator;

    private async void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<LightPackage>>>();
    }

    private void Start()
    {
        StartCoroutine(observerSystemAttributeDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(ObserverSystemAttributeHelper).ToString()), CancellationToken.None));
    }

    public void OnNotify(ObserverSystemAttributeHelper data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        ObserverSubjectDict = data.GetObserverSubjectDict();
    }
}