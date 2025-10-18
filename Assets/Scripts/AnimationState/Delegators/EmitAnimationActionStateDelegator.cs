using System.Collections.Generic;

public class EmitAnimationActionStateDelegator : BaseDelegator<GenericStateBundle<EmitAnimationStateBundle, ActionState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle, ActionState>>>>>();
    }
}
