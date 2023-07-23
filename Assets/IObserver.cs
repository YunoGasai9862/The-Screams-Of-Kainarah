using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public abstract void OnNotify(string stateNotifier, Vector3 Position);
}
