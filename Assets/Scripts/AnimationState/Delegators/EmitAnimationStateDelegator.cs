using System.Collections.Generic;

public class EmitAnimationStateDelegator<T>: BaseDelegator<GenericStateBundle<EmitAnimationStateBundle, T>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle, T>>>>>();
    }
}

public class EmitAnimationMovementStateDelegator : EmitAnimationStateDelegator<MovementState>
{
}

public class EmitAnimationAttackStateDelegator : EmitAnimationStateDelegator<AttackState>
{
}

public class EmitAnimationActionStateDelegator : EmitAnimationStateDelegator<ActionState>
{
}
