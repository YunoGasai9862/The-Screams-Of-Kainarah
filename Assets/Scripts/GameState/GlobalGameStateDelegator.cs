
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

public class GlobalGameStateDelegator: BaseDelegator<GenericStateBundle<GameStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<GenericStateBundle<GameStateBundle>>>>();
    }
}