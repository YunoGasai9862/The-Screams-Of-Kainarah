using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutineWaitForSeconds : RunAsyncCoroutine<WaitForSeconds>
{
    private bool isAttached = false;
    private void Awake()
    {
        if (!isAttached)
            AttachToGameObject();
    }
    async void Update()
    {
        await traverseAsyncOperations();
    }

    public override void AttachToGameObject()
    {
       string className= typeof(RunAsyncCoroutineWaitForSeconds).Name;
       var _ =new GameObject(className).AddComponent<RunAsyncCoroutineWaitForSeconds>();
        isAttached = true;
    }
}
