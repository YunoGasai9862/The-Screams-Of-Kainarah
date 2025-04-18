using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObserverSystemAttributeHelper : MonoBehaviour, ISubject<IObserver<ObserverSystemAttributeHelper>>
{
    [SerializeField]
    public ObserverSystemAttributeDelegator observerSystemAttributeDelegator;

    protected Dictionary<string, List<ObserverSystemAttribute>> ObserverSubjectDict {  get; set; }

    private async void OnEnable()
    {
        ObserverSubjectDict = await Helper.GenerateObserverSystemDict(await Helper.GetGameObjectsWithCustomAttributes<ObserverSystemAttribute>());
    }

    private async void Start()
    {
        observerSystemAttributeDelegator.Subject.SetSubject(this);
    }

    public Dictionary<string, List<ObserverSystemAttribute>> GetObserverSubjectDict()
    {
        return ObserverSubjectDict;
    }

    public void OnNotifySubject(IObserver<ObserverSystemAttributeHelper> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(observerSystemAttributeDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}