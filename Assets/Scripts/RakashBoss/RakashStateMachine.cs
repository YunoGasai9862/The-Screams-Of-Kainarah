using EnemyHittable;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashStateMachine : MonoBehaviour, IObserver<GameState>, IObserver<Player>, IObserver<EnemyHittableManager>
{
    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private const float MAX_DISTANCE_BETWEEN_PLAYER = 15f;

    private const float MIN_DISTANCE_BETWEEN_PLAYER = 3f;

    private GameState GameState { get; set; }

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

    private void Awake()
    {
        GameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        PlayerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();

        Animator= GetComponent<Animator>();

        GameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None);

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()

        }, CancellationToken.None);

        RakashControllerMovement = GetComponent<RakashControllerMovement>();

        RakashMovementCommandController = new Command<MovementActionDelegatePackage, Task<ActionExecuted>>(RakashControllerMovement);

        RakashBattleController = GetComponent<RakashBattleController>();

        RakashBattleCommandController = new Command<BattleActionDelegatePackage, Task<ActionExecuted>>(RakashBattleController);    
    }

    private void Start()
    {
        StartCoroutine(enemyHittableManagerDelegator.NotifySubject(this, new NotificationContext()
        {
            SubjectType = typeof(EnemyHittableManager).ToString(),
            ObserverName = name,
            ObserverTag = tag

        }, CancellationToken.None));
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameState = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
    public void OnNotify(EnemyHittableManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EnemyHittableManager = data;
    }

    protected void CustomOnStateUpdateLogic(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            RakashMovementCommandController.Execute(new MovementActionDelegatePackage()
            {
                MovementAnimationPackage =
                new MovementAnimationPackage { Animation = Animation.STOP_WALK, AnimatorStateInfo = stateInfo, Animator = animator }
            });

            return;
        }

        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("attack"));

        if (Player != null && Helper.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
        {

            RakashMovementCommandController.Execute(new MovementActionDelegatePackage
            {
                MovementAnimationPackage = new MovementAnimationPackage()
                {
                    Animation = Animation.START_WALK,
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
                    Animation = Animation.STOP_ATTACK,
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
                    Animation = Animation.START_ATTACK,
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
                    Animation = Animation.STOP_ATTACK
                },

                AttackActionDelegate = BattleActionDelegate.TAKE_HIT
            });

            _anim.SetTrigger("damage");
            Health -= 10;
        }

        if (Health == 0)
        {
            Vector2 pos = transform.position;
            pos.y = transform.position.y + .5f;
            var deadBody = await HandleBossDefeatScenario(pos, bossDead, gameObject);
            await DestroyMultipleGameObjects(new[] { deadBody, gameObject }, 1f);
        }
    }

}
