
using System.Collections.Generic;
public class PlayerVelocityDelegator : BaseDelegator<CharacterVelocity>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<CharacterVelocity>>>();

        SubjectObserversDict = new Dictionary<string, List<Association<CharacterVelocity>>>(); 
    }
}