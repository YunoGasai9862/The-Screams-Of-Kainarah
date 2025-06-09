using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashFollow : StateMachineBehaviour, IObserver<GameState>, IObserver<Player>
{
    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private const float MAX_DISTANCE_BETWEEN_PLAYER = 15f;

    private const float MIN_DISTANCE_BETWEEN_PLAYER = 3f;

    private GameState GameState { get; set; }

    private Player Player { get; set; }

    private GlobalGameStateDelegator GameStateDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private RakashControllerMovement RakashControllerMovement { get; set; }

    private RakashAttackController RakashAttackController { get; set; }

    private Command<RakashAnimationPackage> RakashMovementCommandController { get; set; }

    private Command<RakashAnimationPackage> RakashAttackCommandController { get; set; }

    private void Awake()
    {
        GameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        PlayerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();

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
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (RakashControllerMovement == null)
        {
            RakashControllerMovement = animator.GetComponent<RakashControllerMovement>();

            RakashMovementCommandController = new Command<RakashAnimationPackage>(RakashControllerMovement);
        }

        if (RakashAttackController == null)
        {
            RakashAttackController = animator.GetComponent<RakashAttackController>();

            RakashAttackCommandController = new Command<RakashAnimationPackage>(RakashAttackController);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            return;
        }

        if (Player != null && HelperFunctions.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
        {
            animator.SetBool("walk", true);
        }

        if (Vector3.Distance(Player.Transform.position, animator.transform.position) <= MIN_DISTANCE_BETWEEN_PLAYER)
        {
            HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
        }
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameState = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
}
