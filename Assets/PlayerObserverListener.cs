using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerObserverListener : MonoBehaviour //try something new (delegates)
{

    private SubjectsToBeNotified<Collider2D, bool> colliderSubjects=new();
    private SubjectsToBeNotified<bool, bool> boolSubjects=new();

    public SubjectsToBeNotified<Collider2D, bool> getColliderSubjects { get => colliderSubjects; }
    public SubjectsToBeNotified<bool, bool> getBoolSubjects { get => boolSubjects; }

    public async Task<bool> PlayerCollisionDelegator(Collider2D collision)
    {
        getColliderSubjects.NotifyObservers(ref collision);
        return await Task.FromResult(true);
    }

    public async Task<bool> PlayerMusicDelegator(bool shouldPlayMusic)
    {
        getBoolSubjects.NotifyObservers(ref shouldPlayMusic);
        return await Task.FromResult(true);
    }

}
