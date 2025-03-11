using System.Collections.Generic;

public class LightFlickerPreprocessDelegator: BaseDelegator<LightFlicker, ILightPreprocess>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Subject<IObserver<LightFlicker, ILightPreprocess>>>();
    }
}