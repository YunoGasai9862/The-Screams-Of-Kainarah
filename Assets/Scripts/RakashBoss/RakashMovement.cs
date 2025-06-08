using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashMovement : StateMachineBehaviour, IObserver<GameState>, IObserver<Player>
{
    private const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private GameState GameState { get; set; }

    private Player Player { get; set; }

    private GlobalGameStateDelegator GameStateDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

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


    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            if (Player != null && HelperFunctions.CheckDistance(animator, 15f, 3f, Player.Transform))
            {
                Vector3 newPos = Player.Transform.position;

                newPos.y = Player.Transform.position.y - 1.5f;

                animator.transform.position = Vector3.MoveTowards(animator.transform.position, newPos, 4f * Time.deltaTime);
            }
            else
            {
                animator.SetBool("walk", false);

                HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
            }

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

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
