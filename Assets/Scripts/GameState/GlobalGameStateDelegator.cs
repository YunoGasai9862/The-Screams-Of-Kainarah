using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GlobalGameStateDelegator: BaseDelegatorEnhanced<GameState>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GameState>>>>();
    }
}