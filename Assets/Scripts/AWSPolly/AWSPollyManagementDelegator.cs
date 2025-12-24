using System.Collections.Generic;
using System.Threading;

public class AWSPollyManagementDelegator: BaseDelegator<IAWSPolly>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IAWSPolly>>>();
    }
}