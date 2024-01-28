using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReceiver<T>
{
    T PerformAction(T value = default);
    T CancelAction();
}
