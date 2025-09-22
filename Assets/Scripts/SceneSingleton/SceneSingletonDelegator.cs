using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SceneSingletonDelegator : BaseDelegator<SceneSingleton>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<SceneSingleton>>>>();
    }
}