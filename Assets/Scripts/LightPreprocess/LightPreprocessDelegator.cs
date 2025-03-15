using System.Collections.Generic;

public class LightPreprocessDelegator<T>: BaseDelegator<T, ILightPreprocess>
{
    //we'll probably need to tune it a little a bit since this wont get exposed via inspector, but a very good approach, now the DICT can be used perfectly
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, SubjectNotifier<IObserver<T, ILightPreprocess>>>();
    }
}