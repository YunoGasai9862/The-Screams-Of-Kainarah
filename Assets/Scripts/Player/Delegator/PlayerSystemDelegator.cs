
using System.Collections.Generic;
public class PlayerSystemDelegator: BaseDelegatorEnhanced<PlayerSystem>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<PlayerSystem>>>>();
    }
}