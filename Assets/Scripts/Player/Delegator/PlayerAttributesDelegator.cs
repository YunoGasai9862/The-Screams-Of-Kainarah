using System.Collections.Generic;
using System.Threading;

public class PlayerAttributesDelegator : BaseDelegator<Player>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<Player>>>>();
    }
}