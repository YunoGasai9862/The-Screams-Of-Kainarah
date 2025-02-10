using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Asset(AssetType = Asset.MONOBEHAVIOR, AddressLabel = "AsyncCoroutine")]
public class AsyncCoroutine : MonoBehaviour, IAsyncCoroutine<WaitForSeconds>, ISubject<IObserver<AsyncCoroutine>>
{
    private AsyncCoroutineDelegator m_asyncCoroutineDelegator;
    private void Start()
    {
        m_asyncCoroutineDelegator = Helper.GetDelegator<AsyncCoroutineDelegator>();
    }

    public async Task ExecuteAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncCoroutine)
    {
        while (await asyncCoroutine.MoveNextAsync())
        {
            await Task.Yield();
        }
    }

    public void OnNotifySubject(IObserver<AsyncCoroutine> data, params object[] optional)
    {
        StartCoroutine(m_asyncCoroutineDelegator.NotifyObserver(data, this));
    }
}