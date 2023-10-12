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
        enemyObserverPattern = FindObjectOfType<EnemyObserverPattern>();
    }

    public async Task<bool> EnemyActionDelegator<Z>(Collider2D collider, GameObject enemyObject, Z value)
    {
        enemyObserverPattern.enemyGameObject = enemyObject; //sets it for use    
        getenemyColliderSubjects.NotifyObservers(ref collider, value);
        return await Task.FromResult(true);
    }

}
