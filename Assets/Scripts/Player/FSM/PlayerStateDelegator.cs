using System.Collections.Generic;

public class PlayerStateDelegator: BaseDelegatorEnhanced<GenericStateBundle<PlayerStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<PlayerStateBundle>>>>>();
    }
}   