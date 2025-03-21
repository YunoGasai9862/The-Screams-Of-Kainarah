using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegator<T>: BaseDelegator<T, ILightPreprocess>
{
    public LightPreprocessDelegator()
    {
        SubjectsDict = new Dictionary<string, SubjectNotifier<IObserver<T, ILightPreprocess>>>();
    }
}