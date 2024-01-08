using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabController : MonoBehaviour
{
    private IReceiver _ledgeGrab;
    private static bool _isGrabbing;
    private Command _ledgeGrabCommand;
    public static bool IsGrabbing { get => _isGrabbing; set => _isGrabbing = value; }

    private void Start()
    {
        _ledgeGrab = GetComponent<IReceiver>();
        _ledgeGrabCommand = new Command(_ledgeGrab);
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
