using System.Collections.Generic;

//NEED TO RESOLVE THIS ----!
public class EmitAnimationStateDelegator: BaseDelegator<GenericStateBundle<EmitAnimationStateBundle>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle>>>>>();
    }
}