using System.Collections.Generic;

public class AWSPollyManagementDelegator: BaseDelegator<IAWSPolly>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<IAWSPolly>>>>();
    }
}