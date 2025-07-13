using System.Collections.Generic;

public class PlayerStateDelegator: BaseDelegatorEnhanced<GenericState<PlayerState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericState<PlayerState>>>>>();
    }
}   