using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private bool _shouldPlay;
    private GameObjectInstantiator _gameObjectCreator;
    private GameObject _enemyGameObject;
    private EnemyAnimationScriptableObject _enemyAnimationScriptableObject;

    public GameObject enemyGameObject { get => _enemyGameObject; set=>_enemyGameObject = value;}

    private void Awake()
    {
        _stateTracker = new AnimationStateMachine(animator);
        _gameObjectCreator = new GameObjectInstantiator(Hit);
        enemyActionDictionary = new Dictionary<string, System.Action>()
        {
            {"Sword",  playHitAnimation},
            {"Dagger", playHitAnimation},
            {"Heroine", attackLogicInitiation }


        };
    }
   
    private void playHitAnimation()
    {
        _stateTracker.AnimationPlayMachineBool(animationHitParam, _shouldPlay);
        handleGameObjectCreation();

    }

    private void attackLogicInitiation()
    {
        //use scriptable Object here
        //use -180 flip so all the colliders flip too!!
    }

    private async void handleGameObjectCreation()
    {
        _gameObjectCreator.InstantiateGameObject(_enemyGameObject.transform.position, Quaternion.identity);
        await Task.Delay(1000);
        _gameObjectCreator.DestroyGameObject(0f);
    }
    public void OnNotify(ref Collider2D Data, params int[] optional)
    {
        foreach (var actionToBePerformed in enemyActionDictionary.Keys)
        {
            if(Data.tag== actionToBePerformed)
            {
                System.Action action = enemyActionDictionary[Data.tag]; //get the function (action) name

                if (!checkIfThereAreMoreThanOneExtraParam(optional))
                {
                    _shouldPlay = optional[0] == 1 ? true : false;
                }
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

    private bool checkIfThereAreMoreThanOneExtraParam(int[] optional)
    {
       return optional.Length>1? true: false;
    }

 
}
