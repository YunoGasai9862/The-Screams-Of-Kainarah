using System;
using System.Collections;
using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<MovementAnimationPackage, ActionExecuted>
{
    private void Start()
    {
        
    }

    ActionExecuted IReceiver<MovementAnimationPackage, ActionExecuted>.PerformAction(MovementAnimationPackage value)
    {
        return new ActionExecuted { };
    }

    public ActionExecuted CancelAction()
    {
        return new ActionExecuted { };
    }
}