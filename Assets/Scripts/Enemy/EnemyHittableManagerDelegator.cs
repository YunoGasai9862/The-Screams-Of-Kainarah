using System.Collections.Generic;

public class EnemyHittableManagerDelegator: BaseDelegator<EnemyHittableManager>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<EnemyHittableManager>>>();

        SubjectObserversDict = new Dictionary<string, List<Association<EnemyHittableManager>>>();
    }
}