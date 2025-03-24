using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegatorEnhanced<ILightPreprocess>
{
    public LightPreprocessDelegator()
    {
        SubjectsDict = new Dictionary<string, SubjectNotifier<IObserverEnhanced<ILightPreprocess>>>();
    }
}