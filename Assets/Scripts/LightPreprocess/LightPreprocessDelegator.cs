using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegatorEnhanced<ILightPreprocess>
{
    private async void OnEnable()
    {
        SubjectsDict = new Dictionary<string, SubjectNotifier<IObserverEnhanced<ILightPreprocess>>>();

        ObserverSystem = await Helper.GetGameObjectsWithCustomAttribute<ObserverSystemAttribute>();

    }
}