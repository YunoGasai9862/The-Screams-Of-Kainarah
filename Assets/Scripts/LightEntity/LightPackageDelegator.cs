using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class LightPackageDelegator: BaseDelegatorEnhanced<LightPackage>, IObserver<DelegatorManager>
{
    private async void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<LightPackage>>>();

        //ObserverSubjectDict = ;
    }
    public void OnNotify(DelegatorManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}