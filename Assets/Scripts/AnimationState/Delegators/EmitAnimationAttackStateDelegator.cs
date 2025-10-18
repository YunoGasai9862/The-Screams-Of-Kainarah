using System.Collections.Generic;

public class EmitAnimationAttackStateDelegator : BaseDelegator<GenericStateBundle<EmitAnimationStateBundle, AttackState>>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle, AttackState>>>>>();
    }
}

