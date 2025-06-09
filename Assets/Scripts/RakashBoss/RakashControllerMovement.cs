using System.Collections;
using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<RakashAnimationPackage>
{
    private void Start()
    {
        
    }

    public RakashAnimationPackage PerformAction(RakashAnimationPackage value = null)
    {
        throw new System.NotImplementedException();
    }

    RakashAnimationPackage IReceiver<RakashAnimationPackage>.CancelAction()
    {
        throw new System.NotImplementedException();
    }
}