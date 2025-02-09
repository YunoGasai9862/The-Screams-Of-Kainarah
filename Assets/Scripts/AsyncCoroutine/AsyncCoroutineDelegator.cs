public class AsyncCoroutineDelegator: BaseDelegator<AsyncCoroutine>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<AsyncCoroutine>>();
    }
}