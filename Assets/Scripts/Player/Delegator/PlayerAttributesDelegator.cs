using System.Collections.Generic;

public class PlayerAttributesDelegator : BaseDelegatorEnhanced<Player>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<Player>>>>();
    }
}