using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyObserverListener : MonoBehaviour
{
    private SubjectsToBeNotified<Collider2D, int> enemyColliderSubjects=new();
    private EnemyObserverPattern enemyObserverPattern;

    public SubjectsToBeNotified<Collider2D, int> getenemyColliderSubjects {get=> enemyColliderSubjects;}

    private void Awake()
    {
        enemyObserverPattern = FindObjectOfType<EnemyObserverPattern>();
    }

    public async Task<bool> EnemyActionDelegator(Collider2D collider, GameObject enemyObject, params int[] optionalints)
    {
        enemyObserverPattern.enemyGameObject = enemyObject; //sets it for use    
        getenemyColliderSubjects.NotifyObservers(ref collider, optionalints);
        return await Task.FromResult(true);
    }


}
