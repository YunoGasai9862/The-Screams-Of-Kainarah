using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegator<ILightPreprocess>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<ILightPreprocess>>>>();
    }
}