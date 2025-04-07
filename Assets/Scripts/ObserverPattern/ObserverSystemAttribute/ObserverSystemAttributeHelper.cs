using System.Collections.Generic;
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

    public void OnNotifySubject(IObserver<ObserverSystemAttributeHelper> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(observerSystemAttributeDelegator.NotifyObserver(data, this));
    }

    public Dictionary<string, List<ObserverSystemAttribute>> GetObserverSubjectDict()
    {
        return ObserverSubjectDict;
    }
}