using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RunAsyncCoroutine : MonoBehaviour
{
    public static void StartAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncCoroutine)
    {
        async IEnumerator runningAsyncCouroutine(IAsyncEnumerator<WaitForSeconds> enumerator)
        {
            while (await enumerator.MoveNextAsync())
            {
                yield return enumerator.Current;
            }

        }

    }


}