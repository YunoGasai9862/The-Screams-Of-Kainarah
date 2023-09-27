using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserverPattern : MonoBehaviour, IObserver<Collider2D>
{
    [SerializeField] EnemyObserverListener _observerScript;
    public void OnNotify(ref Collider2D Data)
    {
        //this function will be called
    }

}
