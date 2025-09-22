using System.Collections.Generic;

public class HealthDelegator: BaseDelegator<Health>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<Health>>>>();
    }
}