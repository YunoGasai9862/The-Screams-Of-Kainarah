
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FloatDelegator : BaseDelegator<float>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<float>>>>();

        SubjectObserversDict = new Dictionary<string, List<Association<float>>>();
    }
}