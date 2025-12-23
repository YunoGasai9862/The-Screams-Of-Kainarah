
using System.Collections.Generic;
using UnityEngine;

public class AsyncCoroutineDelegator: BaseDelegator<AsyncCoroutine>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary <string, Dictionary<string, Subject<AsyncCoroutine>>>();
    }
}