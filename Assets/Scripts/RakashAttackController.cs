using System.Collections;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<bool>
{
    private void Start()
    {

    }
    public bool CancelAction()
    {
        return true;
    }

    public bool PerformAction(bool value = false)
    {
        return true;
    }
}