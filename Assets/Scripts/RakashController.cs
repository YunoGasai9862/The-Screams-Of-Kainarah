using System.Collections;
using UnityEngine;

public class RakashController : MonoBehaviour, IReceiver<bool>
{
    public bool CancelAction()
    {
        throw new System.NotImplementedException();
    }

    public bool PerformAction(bool value = false)
    {
        throw new System.NotImplementedException();
    }
}