using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutineWaitForSeconds : RunAsyncCoroutineGeneric<WaitForSeconds>
{
    private bool isAttached = false;

    async void Update()
    {
        if (!isAttached)
            AttachToGameObject();

        await traverseAsyncOperations();
    }

    public override void AttachToGameObject()
    {
       string className= typeof(RunAsyncCoroutineWaitForSeconds).Name;
       var _ =new GameObject(className).AddComponent<RunAsyncCoroutineWaitForSeconds>();
        isAttached = true;
    }
}
