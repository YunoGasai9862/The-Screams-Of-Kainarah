
using System.Collections.Generic;
public class PlayerSpeedDelegator: BaseDelegatorEnhanced<CharacterSpeed>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<CharacterSpeed>>>>();
    }
}