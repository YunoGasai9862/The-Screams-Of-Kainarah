using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserverPattern : MonoBehaviour, IObserver<Collider2D, int>
{
    private enum enemyAttack
    {
        Attack1=0, Attack2=1
    }

    [Header("Add the Script that delegates for enemies")]
    [SerializeField] EnemyObserverListener _observerScript;
    [Header("Object to instantiate upon hit")]
    [SerializeField] GameObject Hit;
    [Header("Add Enemy's Animator Component")]
    [SerializeField] Animator animator;
    [Header("Enter hit Param name")]
    [SerializeField] string animationHitParam;


    private Dictionary<string, System.Action> enemyActionDictionary;
    private AnimationStateMachine _stateTracker;

    private void Awake()
    {
        _stateTracker = new AnimationStateMachine(animator); 
        enemyActionDictionary = new Dictionary<string, System.Action>()
        {
            {"Sword", playHitAnimation },
            {"Dagger", playHitAnimation}

        };
    }
   
    private void playHitAnimation()
    {
        _stateTracker.AnimationPlayMachineBool(animationHitParam, true);
    }
    public void OnNotify(ref Collider2D Data, params int[] optional)
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
