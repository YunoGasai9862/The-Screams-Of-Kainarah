using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReceiver
{
    void PerformAction();
    void CancelAction();
}
