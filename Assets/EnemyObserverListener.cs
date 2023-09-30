using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyObserverListener : MonoBehaviour
{
    private SubjectsToBeNotified<Collider2D> enemyColliderSubjects=new();

    public SubjectsToBeNotified<Collider2D> getenemyColliderSubjects {get=> enemyColliderSubjects;}

    public async Task<bool> EnemyCollisionDelegator(Collider2D collider)
    {
        getenemyColliderSubjects.NotifyObservers(ref collider);
        return await Task.FromResult(true);
    }
}
