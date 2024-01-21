using System.Collections;
using UnityEngine;

public class RakashController : MonoBehaviour, IReceiver<bool>
{
    public bool CancelAction()
    {
        return true;
    }

    public bool PerformAction(bool value = false)
    {
        return true;
    }
}