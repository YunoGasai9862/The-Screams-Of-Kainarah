using System.Collections.Generic;

public class EnemyHittableManagerDelegator: BaseDelegatorEnhanced<EnemyHittableManager>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<EnemyHittableManager>>>>();

        SubjectObserversDict = new Dictionary<string, List<Association<EnemyHittableManager>>>();
    }
}