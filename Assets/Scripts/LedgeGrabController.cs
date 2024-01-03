using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabController : MonoBehaviour
{
    private IReceiver ledgeGrab;

    private void Start()
    {
        ledgeGrab = GetComponent<IReceiver>();
    }
    void Update()
    {
        ledgeGrab.PerformAction();
    }
}
