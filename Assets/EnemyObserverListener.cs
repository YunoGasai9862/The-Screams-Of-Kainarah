using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyObserverListener : MonoBehaviour
{
    private SubjectsToBeNotified<Collider2D, int> enemyColliderSubjects=new();

    public SubjectsToBeNotified<Collider2D, int> getenemyColliderSubjects {get=> enemyColliderSubjects;}

    public async Task<bool> EnemyCollisionDelegator(Collider2D collider, params int[] optionalInts)
    {
        getenemyColliderSubjects.NotifyObservers(ref collider, optionalInts);
        return await Task.FromResult(true);
    }
}
