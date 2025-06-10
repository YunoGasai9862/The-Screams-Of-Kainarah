using System.Collections;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<RakashAnimationPackage, ActionExecuted>
{
    private void Start()
    {

    }

    ActionExecuted IReceiver<RakashAnimationPackage, ActionExecuted>.PerformAction(RakashAnimationPackage value)
    {
        return new ActionExecuted { };
    }

    ActionExecuted IReceiver<RakashAnimationPackage, ActionExecuted>.CancelAction()
    {
        return new ActionExecuted { };
    }
}