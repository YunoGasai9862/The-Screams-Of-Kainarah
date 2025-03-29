using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegatorEnhanced<ILightPreprocess>
{
    private async void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<ILightPreprocess>>>();

        ObserverSubjectDict = await Helper.GenerateObserverSystemDict(await Helper.GetGameObjectsWithCustomAttribute<ObserverSystemAttribute>());
    }

}