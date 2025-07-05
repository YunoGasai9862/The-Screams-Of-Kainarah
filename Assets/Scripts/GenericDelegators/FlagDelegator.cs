
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlagDelegator : BaseDelegatorEnhanced<bool>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<bool>>>>();

        SubjectObserversDict = new Dictionary<string, List<Association<bool>>>();
    }
}