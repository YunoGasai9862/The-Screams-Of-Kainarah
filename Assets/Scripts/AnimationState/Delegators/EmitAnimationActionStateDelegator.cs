using System.Collections.Generic;

public class EmitAnimationActionStateDelegator : BaseDelegator<GenericStateBundle<EmitAnimationStateBundle<bool>, ActionState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<GenericStateBundle<EmitAnimationStateBundle<bool>, ActionState>>>>();
    }
}
