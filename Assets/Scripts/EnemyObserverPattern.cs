using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserverPattern : MonoBehaviour, IObserver<Collider2D>
{
    [Header("Add the Script that delegates for enemies")]
    [SerializeField] EnemyObserverListener _observerScript;
    [Header("Object to instantiate upon hit")]
    [SerializeField] GameObject Hit;
    [Header("Add Enemy's Animator Component")]
    [SerializeField] Animator _animator;
    
    private Dictionary<string, System.Action> enemyActionDictionary;
    private AnimationStateMachine _currentState;

    private void Awake()
    {
        _currentState = new AnimationStateMachine(_animator); 
        enemyActionDictionary = new Dictionary<string, System.Action>()
        {
            {"Sword", playHitAnimation },
            {"Dagger", playHitAnimation}

        };
    }
    private void playHitAnimation()
    {
       
    }
    public void OnNotify(ref Collider2D Data)
    {
        foreach(var actionToBePerformed in enemyActionDictionary.Keys)
        {
            if(Data.tag== actionToBePerformed)
            {
                System.Action action = enemyActionDictionary[Data.tag]; //get the function (action) name
                action.Invoke(); //invoke it
            }
        }
    }

    private void OnEnable()
    {
        _observerScript.getenemyColliderSubjects.AddObserver(this);
    }

    private void OnDisable()
    {
        _observerScript.getenemyColliderSubjects.RemoveOberver(this);

    }

}
