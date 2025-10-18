using System.Collections.Generic;

public class EmitAnimationMovementStateDelegator : BaseDelegator<GenericStateBundle<EmitAnimationStateBundle, MovementState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle, MovementState>>>>>();
    }
}