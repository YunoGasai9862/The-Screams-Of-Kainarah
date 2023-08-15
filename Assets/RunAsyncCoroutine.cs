using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutine : MonoBehaviour //attach it to the a GameObject
{
    private static RunAsyncCoroutine instance;

    private readonly Queue<IAsyncEnumerator<WaitForSeconds>> asyncEnumeratorCollection = new();

    private void Awake()
    {
        instance = this;
    }
    public static RunAsyncCoroutine RunAsyncCoroutineInstance { get => instance; } //getter + setter

    public static void RunTheAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncEnumerator, CancellationToken _token)
    {
        if (instance == null)
        {
            AttachToGameObject();
        }

        if (!_token.IsCancellationRequested)
            RunAsyncCoroutineInstance.asyncEnumeratorCollection.Enqueue(asyncEnumerator); //adds it to the Queue
        else
            return;
    }
    public async Task ExecuteAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncCoroutine) //passing fucntion
    {
        while (await asyncCoroutine.MoveNextAsync()) //checks if there is any async operation left in the thread, if there is it yeilds back to the main thread momentarily to keep the performance in check
        {
            await Task.Yield(); //yields the thread back to the unity so it can process any pendings tasks/operations, while the asynchronous operations are being handled.
        }

    }
    public static void AttachToGameObject()
    {
        var coroutineRunner = new GameObject("AsyncCoroutineRunner").AddComponent<RunAsyncCoroutine>();

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