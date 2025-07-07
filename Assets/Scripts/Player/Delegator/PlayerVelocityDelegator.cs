
using System.Collections.Generic;
public class PlayerVelocityDelegator : BaseDelegatorEnhanced<CharacterVelocity>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<CharacterVelocity>>>>();
    }
}