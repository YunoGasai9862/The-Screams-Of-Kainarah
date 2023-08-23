using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutine<T> : MonoBehaviour //attach it to the a GameObject
{
    private readonly static Queue<IAsyncEnumerator<T>> asyncEnumeratorCollection = new();

    private static bool singleInstancePerClass = false;

    public static void RunTheAsyncCoroutine(IAsyncEnumerator<T> asyncEnumerator, T type,  CancellationToken _token)
    {
        if (singleInstancePerClass == false)
        {
            AttachToGameObject(type); 

            singleInstancePerClass = true;
        }

        if (!_token.IsCancellationRequested)
             asyncEnumeratorCollection.Enqueue(asyncEnumerator); //adds it to the Queue
        else
            return;
    }
    public async Task ExecuteAsyncCoroutine(IAsyncEnumerator<T> asyncCoroutine) //passing fucntion
    {
        while (await asyncCoroutine.MoveNextAsync()) //checks if there is any async operation left in the thread, if there is it yeilds back to the main thread momentarily to keep the performance in check
        {
            await Task.Yield(); //yields the thread back to the unity so it can process any pendings tasks/operations, while the asynchronous operations are being handled.
        }

    }
    public static void AttachToGameObject(T type)
    {
        var _ = new GameObject("AsyncCoroutineRunner" + type.GetType()).AddComponent<RunAsyncCoroutine<T>>();

    }

    private async void Update()
    {
        if (asyncEnumeratorCollection.Count > 0) //makes sure other Async fucntions keep running if there are any
        {
            var asyncEnumerator = asyncEnumeratorCollection.Dequeue(); //removes from the queue
            await ExecuteAsyncCoroutine(asyncEnumerator); //executes it
        }
    }

}