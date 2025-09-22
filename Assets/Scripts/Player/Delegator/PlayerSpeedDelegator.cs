
using System.Collections.Generic;
public class PlayerSpeedDelegator: BaseDelegator<CharacterSpeed>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<CharacterSpeed>>>>();
    }
}