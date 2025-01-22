using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EntityPoolManagerDelegator : BaseDelegator<EntityPoolManager>  
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<EntityPoolManager>>();
    }
}