using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncCoroutine : MonoBehaviour, IAsyncCoroutine<WaitForSeconds>
{
    public async Task ExecuteAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncCoroutine)
    {
        while (await asyncCoroutine.MoveNextAsync())
        {
            await Task.Yield();
        }

    }

}