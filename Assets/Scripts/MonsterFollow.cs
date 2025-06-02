using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MonsterFollow : StateMachineBehaviour, IObserver<GameState>
{
    public static GameObject Player;

    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private GameState GameState { get; set; }

    private GlobalGameStateDelegator GameStateDelegator { get; set; }

    private void Awake()
    {
        GameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        GameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //improve later
        if (!GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            if (Player != null && HelperFunctions.CheckDistance(animator, 15f, 3f, Player))
            {
                animator.SetBool("walk", true);
            }

            if (Vector3.Distance(Player.transform.position, animator.transform.position) <= 3)
            {
                HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
            }

        }
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameState = data;
    }
}
