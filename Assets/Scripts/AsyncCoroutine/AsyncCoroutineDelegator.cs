
using UnityEngine;

public class AsyncCoroutineDelegator: BaseDelegator<AsyncCoroutine>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<AsyncCoroutine>>();

        Debug.Log(Subject);
    }
}