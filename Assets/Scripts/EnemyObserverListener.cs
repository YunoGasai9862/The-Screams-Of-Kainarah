using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyObserverListener : MonoBehaviour
{
    private SubjectsToBeNotifiedV2<Collider2D> enemyColliderSubjects=new();
    private EnemyObserverPattern enemyObserverPattern;

    public SubjectsToBeNotifiedV2<Collider2D> getenemyColliderSubjects {get=> enemyColliderSubjects;}

    private void Awake()
    {
        enemyObserverPattern = FindFirstObjectByType<EnemyObserverPattern>();
    }

    public async Task<bool> EnemyActionDelegator<Z, Y>(Collider2D collider, GameObject enemyObject, Z animName, Y animValue)
    {
        enemyObserverPattern.enemyGameObject = enemyObject; //sets it for use    
        getenemyColliderSubjects.NotifyObservers(collider, animName, animValue);
        return await Task.FromResult(true);
    }

}
