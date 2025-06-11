using System.Collections;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<AttackAnimationPackage, ActionExecuted>
{
    private void Start()
    {

    }

    ActionExecuted IReceiver<AttackAnimationPackage, ActionExecuted>.PerformAction(AttackAnimationPackage value)
    {
        return new ActionExecuted { };
    }

    ActionExecuted IReceiver<AttackAnimationPackage, ActionExecuted>.CancelAction()
    {
        return new ActionExecuted { };
    }
}