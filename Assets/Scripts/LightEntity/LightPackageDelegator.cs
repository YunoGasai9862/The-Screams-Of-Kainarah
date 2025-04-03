using System.Collections.Generic;
using UnityEngine;
public class LightPackageDelegator: BaseDelegatorEnhanced<LightPackage>
{
    private async void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<LightPackage>>>();

        ObserverSubjectDict = await Helper.GenerateObserverSystemDict(await Helper.GetGameObjectsWithCustomAttributes<ObserverSystemAttribute>());
    }
}