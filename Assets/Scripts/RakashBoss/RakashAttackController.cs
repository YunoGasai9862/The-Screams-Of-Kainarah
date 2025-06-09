using System.Collections;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<RakashAnimationPackage>
{
    private void Start()
    {

    }
    public RakashAnimationPackage CancelAction()
    {
        throw new System.NotImplementedException();
    }

    public RakashAnimationPackage PerformAction(RakashAnimationPackage value = null)
    {
        throw new System.NotImplementedException();
    }
}