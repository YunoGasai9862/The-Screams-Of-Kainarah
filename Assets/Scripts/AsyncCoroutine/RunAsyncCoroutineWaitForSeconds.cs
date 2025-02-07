using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAsyncCoroutineWaitForSeconds : RunAsyncCoroutineGeneric<WaitForSeconds>
{
    //use some other sort of signaling? why update!
    async void Update()
    {
        await traverseAsyncOperations();
    }

}
