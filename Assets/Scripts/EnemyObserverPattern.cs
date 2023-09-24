using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserverPattern : MonoBehaviour, IObserver<Collider2D>
{
    public void OnNotify(ref Collider2D Data)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
