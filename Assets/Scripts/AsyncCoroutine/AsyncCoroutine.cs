using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Asset(Asset.MONOBEHAVIOR,  "AsyncCoroutine", InstantiationOrder = 9)]
[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(AsyncCoroutine), ContextType = typeof(AsyncCoroutine))]
public class AsyncCoroutine : Scene, IAsyncCoroutine<WaitForSeconds>, IAsyncCoroutine<WaitUntil>, IRequest<AsyncCoroutine>
{
    private Delegator Delegator { get; set; }
    private async void Start()
    {
        StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));
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

    public IEnumerator Request()
    {
       yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<AsyncCoroutine>() { Data = this, EntityType = typeof(AsyncCoroutine) }, this));
    }
}