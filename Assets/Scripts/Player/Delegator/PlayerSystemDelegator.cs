
using System.Collections.Generic;
public class PlayerSystemDelegator: BaseDelegator<PlayerSystem>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<PlayerSystem>>>>();
    }
}