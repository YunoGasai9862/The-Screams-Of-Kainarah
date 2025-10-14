using System.Collections.Generic;

public class EmitAnimationStateDelegator<T>: BaseDelegator<AnimationStateBundle<EmitAnimationStateBundle, IAnimationState<T>>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<AnimationStateBundle<EmitAnimationStateBundle, IAnimationState<T>>>>>>();
    }
}

public class EmitAnimationMovementStateDelegator : EmitAnimationStateDelegator<PlayerMovementState>
{
}

public class EmitAnimationAttackStateDelegator : EmitAnimationStateDelegator<PlayerAttackState>
{
}

public class EmitAnimationActionStateDelegator : EmitAnimationStateDelegator<PlayerActionState>
{
}
