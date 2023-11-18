using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerObserverListener : MonoBehaviour //try something new (delegates)
{

    private SubjectsToBeNotified<Collider2D> colliderSubjects=new();
    private SubjectsToBeNotified<bool> boolSubjects=new();
    private SubjectsToBeNotified<DialogueEntityScriptableObject.DialogueEntity> _entities = new();


    public SubjectsToBeNotified<Collider2D> getColliderSubjects { get => colliderSubjects; }
    public SubjectsToBeNotified<bool> getBoolSubjects { get => boolSubjects; }
    public SubjectsToBeNotified<DialogueEntityScriptableObject.DialogueEntity> getDialogueEntites { get => _entities; }

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

    public async Task<bool> DialogueDelegator(DialogueEntityScriptableObject.DialogueEntity dialogueEntity) //change this to
    {
        getDialogueEntites.NotifyObservers(ref dialogueEntity);
        return await Task.FromResult(true);
    }

}
