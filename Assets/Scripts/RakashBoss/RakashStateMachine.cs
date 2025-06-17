using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashStateMachine : MonoBehaviour, IObserver<GameState>, IObserver<Player>
{
    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private const float MAX_DISTANCE_BETWEEN_PLAYER = 15f;

    private const float MIN_DISTANCE_BETWEEN_PLAYER = 3f;

    protected GameState GameState { get; set; }

    protected Player Player { get; set; }

    protected GlobalGameStateDelegator GameStateDelegator { get; set; }

    protected PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    protected RakashControllerMovement RakashControllerMovement { get; set; }

    protected RakashAttackController RakashAttackController { get; set; }

    protected Command<MovementAnimationPackage, Task<ActionExecuted>> RakashMovementCommandController { get; set; }

    protected Command<AttackAnimationPackage, Task<ActionExecuted>> RakashAttackCommandController { get; set; }

    private Animator Animator { get; set; }

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

        RakashControllerMovement = Animator.GetComponent<RakashControllerMovement>();

        RakashMovementCommandController = new Command<MovementAnimationPackage, Task<ActionExecuted>>(RakashControllerMovement);
 
        RakashAttackController = Animator.GetComponent<RakashAttackController>();

        RakashAttackCommandController = new Command<AttackAnimationPackage, Task<ActionExecuted>>(RakashAttackController);
        
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameState = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }

    protected void CustomOnStateUpdateLogic(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            RakashMovementCommandController.Execute(new MovementAnimationPackage() { Animation = Animation.STOP_WALK, AnimatorStateInfo = stateInfo, Animator = animator });

            return;
        }

        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("attack"));

        if (Player != null && Helper.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
        {
             RakashMovementCommandController.Execute(new MovementAnimationPackage()
            {
                Animation = Animation.START_WALK,
                Animator = animator,
                AnimatorStateInfo = stateInfo,
                MainEntityTransform = transform,
                TargetTransform = Player.Transform
            });
        }

        if (Vector3.Distance(Player.Transform.position, animator.transform.position) <= MIN_DISTANCE_BETWEEN_PLAYER)
        {
            RakashMovementCommandController.Execute(new MovementAnimationPackage()
            {
                Animation = Animation.STOP_ATTACK,
                Animator = animator,
                AnimatorStateInfo = stateInfo,
                MainEntityTransform = transform,
                TargetTransform = Player.Transform
            });

            RakashAttackCommandController.Execute(new AttackAnimationPackage() { Animation = Animation.START_ATTACK, AnimatorStateInfo = stateInfo, Animator = animator, AttackDelay = TIME_SPAN_BETWEEN_EACH_ATTACK });
        }
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (await EnemyHittableManager.IsEntityAnAttackObject(collision, SceneSingleton.EnemyHittableObjects))
        {
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
