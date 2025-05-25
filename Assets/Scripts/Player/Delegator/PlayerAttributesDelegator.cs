using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerAttributesDelegator : BaseDelegatorEnhanced<Transform>
{

    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<Transform>>>>();
    }
}