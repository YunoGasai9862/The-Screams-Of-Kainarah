public class LightProcessorDelegator: BaseDelegator<LightEntity>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<LightEntity>>();
    }
}