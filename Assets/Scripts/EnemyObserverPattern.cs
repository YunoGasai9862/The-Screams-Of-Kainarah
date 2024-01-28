using EnemyAnimation;
using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyObserverPattern : MonoBehaviour, IObserverV2<Collider2D>
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
    [Header("Add the Scriptable Object that Contains the Animation Information")]
    [SerializeField] EnemyAnimationScriptableObject _enemyAnimationScriptableObject;

    private Dictionary<string, System.Action<object, object>> enemyActionDictionary;
    private AnimationStateMachine _stateTracker;
    private GameObjectInstantiator _gameObjectCreator;
    private GameObject _enemyGameObject;
    private int animationPosInTheObject;
    public GameObject enemyGameObject { get => _enemyGameObject; set=>_enemyGameObject = value;}

    private void Awake()
    {
        _stateTracker = new AnimationStateMachine(animator);
        _gameObjectCreator = new GameObjectInstantiator(Hit);
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
        _stateTracker.AnimationPlayForBool((string)animName, _enemyAnimationScriptableObject.eachAnimation[animationPosInTheObject].valueBool);
        HandleGameObjectCreation();
 
    }
    private void AttackLogicInitiation(object animName, object value)
    {
        AnimationFinder(_enemyAnimationScriptableObject, (string)animName, value);
        _stateTracker.AnimationPlayForBool((string)animName, _enemyAnimationScriptableObject.eachAnimation[animationPosInTheObject].valueBool);

    }
    private async void HandleGameObjectCreation()
    {
        _gameObjectCreator.InstantiateGameObject(_enemyGameObject.transform.position, Quaternion.identity);
        await Task.Delay(1000);
        _gameObjectCreator.DestroyGameObject(0f);
    }
    public void OnNotify<Z, Y>(Collider2D Data, Z value1, Y value2)
    {
        if(enemyActionDictionary.TryGetValue(Data.tag, out var func))  //put it in the func
        {
            func.Invoke(value1, value2);//similar to doing it separately
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


}
