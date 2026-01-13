using EnemyHittable;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashStateMachine : MonoBehaviour, IObserver<GenericStateBundle<GameStateBundle>>, IObserver<IEntityTransform>, IObserver<EnemyHittableManager>
{
    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private const float MAX_DISTANCE_BETWEEN_PLAYER = 15f;

    private const float MIN_DISTANCE_BETWEEN_PLAYER = 3f;

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private Player Player { get; set; }

    private GlobalGameStateDelegator GameStateDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private RakashControllerMovement RakashControllerMovement { get; set; }

    private RakashBattleController RakashBattleController { get; set; }

    private EnemyHittableManager EnemyHittableManager { get; set; }

    private Command<MovementActionDelegatePackage, Task<ActionExecuted>> RakashMovementCommandController { get; set; }

    private Command<BattleActionDelegatePackage, Task<ActionExecuted>> RakashBattleCommandController { get; set; }

    private Animator Animator { get; set; }

    [SerializeField]
    EnemyHittableObjects enemyHittableObjects;
    [SerializeField]
    EnemyHittableManagerDelegator enemyHittableManagerDelegator;

    private async void Awake()
    {
        GameStateDelegator = await Helper.GetDelegator<GlobalGameStateDelegator>();

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        Animator= GetComponent<Animator>();

        GameStateDelegator.NotifySubjectWrapper(this, new ObserverContext()
        {
            Instance = gameObject,
            SubjectType = typeof(GameStateConsumer)

        }, CancellationToken.None);

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new ObserverContext()
        {
            Instance = gameObject,
            SubjectType = typeof(PlayerAttributesNotifier)

        }, CancellationToken.None);

        RakashControllerMovement = GetComponent<RakashControllerMovement>();

        RakashMovementCommandController = new Command<MovementActionDelegatePackage, Task<ActionExecuted>>(RakashControllerMovement);

        RakashBattleController = GetComponent<RakashBattleController>();

        RakashBattleCommandController = new Command<BattleActionDelegatePackage, Task<ActionExecuted>>(RakashBattleController);    
    }

    private void Start()
    {
        StartCoroutine(enemyHittableManagerDelegator.NotifySubject(this, new ObserverContext()
        {
            SubjectType = typeof(EnemyHittableManager),
            Instance = gameObject,

        }, CancellationToken.None));
    }

    public void OnNotify(GenericStateBundle<GameStateBundle> data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState.StateBundle = data.StateBundle;
    }

    public void OnNotify(IEntityTransform data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player.Transform = data.Transform;
    }
    public void OnNotify(EnemyHittableManager data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EnemyHittableManager = data;
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

        if (Player != null && Helper.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
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
}
