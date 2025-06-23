using System.Collections.Generic;

public class HealthDelegator: BaseDelegatorEnhanced<Health>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<Health>>>>();
    }
}