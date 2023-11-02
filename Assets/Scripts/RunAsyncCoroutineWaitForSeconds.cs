using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutineWaitForSeconds : RunAsyncCoroutineGeneric<WaitForSeconds>
{
    public static bool isAttached=false;

    public static bool GetIsAttached {get => isAttached;}

    public static new void AttachToGameObject()  //why new? => it overrides the nonstatic instance, and use this one instead.
    {
       string className= typeof(RunAsyncCoroutineWaitForSeconds).Name;
       var _ =new GameObject(className).AddComponent<RunAsyncCoroutineWaitForSeconds>();
        isAttached = true;
    }

    async void Update()
    {
        await traverseAsyncOperations();
    }

}
