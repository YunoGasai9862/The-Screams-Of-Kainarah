using System;
using System.Collections;
using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<RakashAnimationPackage, ActionExecuted>
{
    private void Start()
    {
        
    }

    ActionExecuted IReceiver<RakashAnimationPackage, ActionExecuted>.PerformAction(RakashAnimationPackage value)
    {
        return new ActionExecuted { };
    }

    public ActionExecuted CancelAction()
    {
        return new ActionExecuted { };
    }
}