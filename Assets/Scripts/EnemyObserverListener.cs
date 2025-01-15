using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyObserverListener : MonoBehaviour
{
    private ObserverExtended<Collider2D, bool, string> enemyColliderSubjects=new();
    private EnemyObserverPattern enemyObserverPattern;

    public ObserverExtended<Collider2D, bool, string> getenemyColliderSubjects {get=> enemyColliderSubjects;}

    private void Awake()
    {
        enemyObserverPattern = FindFirstObjectByType<EnemyObserverPattern>();
    }

    public async Task<bool> EnemyActionDelegator(Collider2D collider, GameObject enemyObject, string animName, bool animValue)
    {
        enemyObserverPattern.enemyGameObject = enemyObject; //sets it for use    
        getenemyColliderSubjects.NotifyObservers(collider, animName, animValue);
        return await Task.FromResult(true);
    }

}
