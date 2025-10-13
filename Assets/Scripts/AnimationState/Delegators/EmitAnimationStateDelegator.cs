using System.Collections.Generic;

public class EmitAnimationStateDelegator: BaseDelegator<AnimationStateBundle<EmitAnimationStateBundle, IAnimationState<>>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<AnimationStateBundle<EmitAnimationStateBundle, IAnimationState<>>>>>>();
    }
}