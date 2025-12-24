using System.Collections.Generic;

public class PlayerStateDelegator: BaseDelegator<GenericStateBundle<PlayerStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<GenericStateBundle<PlayerStateBundle>>>>();
    }
}   