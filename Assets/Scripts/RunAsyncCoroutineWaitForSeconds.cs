using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutineWaitForSeconds : RunAsyncCoroutineGeneric<WaitForSeconds>
{
    public bool isAttached;

    public bool GetIsAttached {get => isAttached; set => isAttached=value;}

    public RunAsyncCoroutineWaitForSeconds()
    {
        isAttached = false;
    }
    public override void AttachToGameObject()
    {
       string className= typeof(RunAsyncCoroutineWaitForSeconds).Name;
       var _ =new GameObject(className).AddComponent<RunAsyncCoroutineWaitForSeconds>();
        isAttached = true;
    }
}
