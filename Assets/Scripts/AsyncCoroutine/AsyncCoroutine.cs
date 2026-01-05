using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[Asset(Asset.MONOBEHAVIOR,  "AsyncCoroutine", InstantiationOrder = 5)]
public class AsyncCoroutine : MonoBehaviour, IAsyncCoroutine<WaitForSeconds>, IAsyncCoroutine<WaitUntil>, ISubject<AsyncCoroutine>
{
    private AsyncCoroutineDelegator m_asyncCoroutineDelegator;
    private async void Start()
    {
        m_asyncCoroutineDelegator = await Helper.GetDelegator<AsyncCoroutineDelegator>();

        m_asyncCoroutineDelegator.AddToSubjectsDict(typeof(AsyncCoroutine).ToString(), name, new Subject<AsyncCoroutine>(this, typeof(AsyncCoroutine)));
    }

    public async Task ExecuteAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncCoroutine)
    {
        while (await asyncCoroutine.MoveNextAsync())
        {
            await Task.Yield();
        }
    }

    public async Task ExecuteAsyncCoroutine(IAsyncEnumerator<WaitUntil> asyncCoroutine)
    {
        while (await asyncCoroutine.MoveNextAsync()) //checks if the coroutine i have passed has an element/next item to process, if so it yields it (with await)
        {
            await Task.Yield();
        }
    }

    public void OnNotifySubject(IObserver<AsyncCoroutine> data, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(m_asyncCoroutineDelegator.NotifyObserver(data, this, context, cancellationToken: cancellationToken));
    }
}