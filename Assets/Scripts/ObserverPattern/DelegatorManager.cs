using System.Collections.Generic;
using UnityEngine;

public class DelegatorManager: MonoBehaviour, ISubject<IObserver<DelegatorManager>>
{
     public Dictionary<string, List<ObserverSystemAttribute>> ObserverSubjectDict {  get; private set; }

    private async void Start()
    {
        ObserverSubjectDict = await Helper.GenerateObserverSystemDict(await Helper.GetGameObjectsWithCustomAttributes<ObserverSystemAttribute>());
    }

    public void OnNotifySubject(IObserver<DelegatorManager> data, NotificationContext notificationContext, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}