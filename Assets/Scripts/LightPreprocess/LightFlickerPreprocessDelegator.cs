public class LightFlickerPreprocessDelegator: BaseDelegator<LightFlicker, ILightPreprocess>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<LightFlicker, ILightPreprocess>>();
    }
}