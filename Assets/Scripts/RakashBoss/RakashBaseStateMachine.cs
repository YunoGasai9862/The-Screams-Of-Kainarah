using System.Threading;
using UnityEngine;

public class RakashBaseStateMachine : MonoBehaviour, IObserver<GameState>, IObserver<Player>
{
    protected GameState GameState { get; set; }

    protected Player Player { get; set; }

    protected GlobalGameStateDelegator GameStateDelegator { get; set; }

    protected PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    protected RakashControllerMovement RakashControllerMovement { get; set; }

    protected RakashAttackController RakashAttackController { get; set; }

    protected Command<MovementAnimationPackage, Vector3> RakashMovementCommandController { get; set; }

    protected Command<AttackAnimationPackage, ActionExecuted> RakashAttackCommandController { get; set; }

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

            RakashMovementCommandController = new Command<MovementAnimationPackage, Vector3>(RakashControllerMovement);
        }

        if (RakashAttackController == null)
        {
            RakashAttackController = animator.GetComponent<RakashAttackController>();

            RakashAttackCommandController = new Command<AttackAnimationPackage, ActionExecuted>(RakashAttackController);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CustomOnStateUpdateLogic(animator, stateInfo, layerIndex);
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameState = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }

    protected abstract void CustomOnStateUpdateLogic(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
}
