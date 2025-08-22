using System.Collections.Generic;

public class EmitAnimationStateDelegator: BaseDelegatorEnhanced<GenericStateBundle<EmitAnimationStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle>>>>>();
    }
}