using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegator<T>: BaseDelegator<T, ILightPreprocess> where T: MonoBehaviour
{
    //we'll probably need to tune it a little a bit since this wont get exposed via inspector, but a very good approach, now the DICT can be used perfectly
    public LightPreprocessDelegator()
    {
        SubjectsDict = new Dictionary<string, SubjectNotifier<IObserver<T, ILightPreprocess>>>();
    }

}