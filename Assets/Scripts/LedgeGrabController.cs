using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabController : MonoBehaviour
{
    private IReceiver<bool> _ledgeGrab;
    private Command<bool> _ledgeGrabCommand;

    private void Start()
    {
        _ledgeGrab = GetComponent<IReceiver<bool>>();
        _ledgeGrabCommand = new Command<bool>(_ledgeGrab);
    }
    public void PerformLedgeGrab()
    {
        _ledgeGrabCommand.Execute();
    }

    public void CancelLedgeGrab()
    {
        _ledgeGrabCommand.Cancel();
    }
}
