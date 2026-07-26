using Assets.Annotations;
using EnemyHittable;
using System.Collections;
using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.Scene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(RakashStateMachine), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(RakashStateMachine), SubjectType = typeof(EnemyHittableManager), ContextType = typeof(EnemyHittableManager))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(RakashStateMachine), SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
public class RakashStateMachine : Scene, INotify<GenericStateBundle<GameStateBundle>>, INotify<IEntityTransform>, INotify<EnemyHittableManager>
{
    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private const float MAX_DISTANCE_BETWEEN_PLAYER = 15f;

    private const float MIN_DISTANCE_BETWEEN_PLAYER = 3f;

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private Player Player { get; set; }

    private Delegator Delegator { get; set; }

    private RakashControllerMovement RakashControllerMovement { get; set; }

    private RakashBattleController RakashBattleController { get; set; }

    private EnemyHittableManager EnemyHittableManager { get; set; }

    private Command<MovementActionDelegatePackage, Task<ActionExecuted>> RakashMovementCommandController { get; set; }

    private Command<BattleActionDelegatePackage, Task<ActionExecuted>> RakashBattleCommandController { get; set; }

    private Animator Animator { get; set; }

    private SceneUtils SceneUtils { get; set; }

    [SerializeField]
    EnemyHittableObjects enemyHittableObjects;

    private async void Awake()
    {
        SceneUtils = await BaseScene.GetSceneUtilsAsync();

        StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        Animator = GetComponent<Animator>();

        RakashControllerMovement = GetComponent<RakashControllerMovement>();

        RakashMovementCommandController = new Command<MovementActionDelegatePackage, Task<ActionExecuted>>(RakashControllerMovement);

        RakashBattleController = GetComponent<RakashBattleController>();

        RakashBattleCommandController = new Command<BattleActionDelegatePackage, Task<ActionExecuted>>(RakashBattleController);    
    }

    private void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<GenericStateBundle<GameStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(RakashStateMachine),
            SubjectType = typeof(GameStateConsumer)

        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<IEntityTransform>()
        {
            Instance = gameObject,
            EntityType = typeof(RakashStateMachine),
            SubjectType = typeof(PlayerAttributesNotifier)

        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<EnemyHittableManager>()
        {
            SubjectType = typeof(EnemyHittableManager),
            EntityType = typeof(RakashStateMachine),
            Instance = gameObject,

        }, this));
    }

    protected void CustomOnStateUpdateLogic(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            RakashMovementCommandController.Execute(new MovementActionDelegatePackage()
            {
                MovementAnimationPackage =
                new MovementAnimationPackage { Animations = new System.Collections.Generic.List<Animation>() { Animation.STOP_WALK }, AnimatorStateInfo = stateInfo, Animator = animator }
            });

            return;
        }

        if (Player != null && SceneUtils.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
        {

            RakashMovementCommandController.Execute(new MovementActionDelegatePackage
            {
                MovementAnimationPackage = new MovementAnimationPackage()
                {
                    Animations = new System.Collections.Generic.List<Animation>() { Animation.START_WALK },
                    Animator = animator,
                    AnimatorStateInfo = stateInfo,
                    MainEntityTransform = transform,
                    TargetTransform = Player.Transform

                }
            });
        }

        if (Vector3.Distance(Player.Transform.position, animator.transform.position) <= MIN_DISTANCE_BETWEEN_PLAYER)
        {
            RakashMovementCommandController.Execute(new MovementActionDelegatePackage
            {
                MovementAnimationPackage = new MovementAnimationPackage()
                {
                    Animations = new System.Collections.Generic.List<Animation>() { Animation.STOP_ATTACK },
                    Animator = animator,
                    AnimatorStateInfo = stateInfo,
                    MainEntityTransform = transform,
                    TargetTransform = Player.Transform
                }
            });

            RakashBattleCommandController.Execute(new BattleActionDelegatePackage
            {
                AttackAnimationPackage = new AttackAnimationPackage()
                {
                    Animations = new System.Collections.Generic.List<Animation>() { Animation.START_ATTACK },
                    AnimatorStateInfo = stateInfo,
                    Animator = animator,
                    AttackDelay = TIME_SPAN_BETWEEN_EACH_ATTACK
                }
            });
        }
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (await EnemyHittableManager.IsEntityAnAttackObject(collision, enemyHittableObjects))
        {
            await RakashBattleCommandController.Execute(new BattleActionDelegatePackage
            {
                AttackAnimationPackage = new AttackAnimationPackage()
                {
                    Animations = new System.Collections.Generic.List<Animation>() { Animation.STOP_ATTACK, Animation.TAKE_HIT }
                },

                AttackActionDelegate = BattleActionDelegate.TAKE_HIT
            });
        }
    }


    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        CurrentGameState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(IEntityTransform value)
    {
        Player.Transform = value.Transform;

        yield return null;
    }

    public IEnumerator Notify(EnemyHittableManager value)
    {
        EnemyHittableManager = value;

        yield return null;
    }
}
