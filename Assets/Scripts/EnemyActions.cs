using Assets.Annotations;
using Assets.Scripts.Enemy.Models;
using EnemyAnimation;
using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(EnemyScript), EntityType = typeof(EnemyActions), ContextType = typeof(EnemyActionBundle))]
public class EnemyActions : Scene, INotify<EnemyActionBundle>
{
    private enum enemyAttack
    {
        Attack1=0, Attack2=1
    }

    [Header("Object to instantiate upon hit")]
    [SerializeField] GameObject Hit;
    [Header("Add Enemy's Animator Component")]
    [SerializeField] Animator animator;
    [Header("Add the Scriptable Object that Contains the Animation Information")]
    [SerializeField] EnemyAnimationScriptableObject _enemyAnimationScriptableObject;

    private Dictionary<string, System.Action<object, object>> enemyActionDictionary;
    private AnimationStateMachine _stateTracker;
    private InstantiateUtility _gameObjectCreator;
    private GameObject _enemyGameObject;
    private int animationPosInTheObject;

    public GameObject enemyGameObject { get => _enemyGameObject; set=>_enemyGameObject = value;}

    private void Awake()
    {
        _stateTracker = new AnimationStateMachine(animator);
        _gameObjectCreator = new InstantiateUtility(Hit);
        enemyActionDictionary = new Dictionary<string, System.Action<object, object>>() //object is required here
        {
            {"Sword",  (animName,  value) => PlayHitAnimation(animName, value)}, //lambda expression for passing values
            {"Dagger", (animName,  value) => PlayHitAnimation(animName, value)},
            {"Player", (animName,  value) => AttackLogicInitiation(animName, value)}

        };
    }

    private void PlayHitAnimation(object animName, object value)
    {
        AnimationFinder(_enemyAnimationScriptableObject, (string)animName, value);
        _stateTracker.SetAnimation((string)animName, _enemyAnimationScriptableObject.eachAnimation[animationPosInTheObject].valueBool);
        HandleGameObjectCreation();
 
    }
    private void AttackLogicInitiation(object animName, object value)
    {
        AnimationFinder(_enemyAnimationScriptableObject, (string)animName, value);
        _stateTracker.SetAnimation((string)animName, _enemyAnimationScriptableObject.eachAnimation[animationPosInTheObject].valueBool);

    }
    private async void HandleGameObjectCreation()
    {
        _gameObjectCreator.InstantiateObject(_enemyGameObject.transform.position, Quaternion.identity);
        await Task.Delay(1000);
        _gameObjectCreator.DestroyObjectAfter();
    }

    private void AnimationFinder<T>(EnemyAnimationScriptableObject enemy, string paramToSearch, T valueToSet)
    {
        for(int i=0; i< enemy.eachAnimation.Length; i++)  
        {
            if (paramToSearch == enemy.eachAnimation[i].animationName)
            {
                animationPosInTheObject = i;
                switch (valueToSet)  //c# pattern matching algorithm
                {
                    case int intValue:
                        enemy.eachAnimation[i].valueInt = intValue;
                        break;
                    case bool boolValue:
                        enemy.eachAnimation[i].valueBool = boolValue;
                        break;
                    case float floatValue:
                        enemy.eachAnimation[i].valueFloat= floatValue;
                        break;      
                    case string stringValue:
                        enemy.eachAnimation[i].valueString = stringValue;
                        break;
                    default:
                        break;

                }
            }
        }
    }
    public IEnumerator Notify(EnemyActionBundle value)
    {
        if (enemyActionDictionary.TryGetValue(value.ActionName, out var func))
        {
            func.Invoke(value.ActionName, value.ActionValue);
        }
        
        yield return null;
    }
}
