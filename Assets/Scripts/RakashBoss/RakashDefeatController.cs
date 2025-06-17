using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RakashDefeatController : MonoBehaviour, IReceiver<AttackAnimationPackage, Task<ActionExecuted>>
{
    public Task<ActionExecuted> CancelAction()
    {
        throw new System.NotImplementedException();
    }

    public Task<ActionExecuted> PerformAction(AttackAnimationPackage value = null)
    {
        throw new System.NotImplementedException();
    }
} 