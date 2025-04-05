using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegatorEnhanced<ILightPreprocess>, IObserver<DelegatorManager>
{
    private async void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<ILightPreprocess>>>();

        //ObserverSubjectDict = await Helper.GenerateObserverSystemDict(await Helper.GetGameObjectsWithCustomAttributes<ObserverSystemAttribute>());
    }

    public void OnNotify(DelegatorManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}