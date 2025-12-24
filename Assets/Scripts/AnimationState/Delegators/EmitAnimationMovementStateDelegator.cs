using System.Collections.Generic;

public class EmitAnimationMovementStateDelegator : BaseDelegator<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>>>();
    }
}