public class ObserverSystemAttributeDelegator: BaseDelegator<ObserverSystemAttributeHelper>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<ObserverSystemAttributeHelper>>();
    }
}