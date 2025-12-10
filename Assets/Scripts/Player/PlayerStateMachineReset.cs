using UnityEngine;

public class PlayerStateMachineReset: StateMachineBehaviour
{
    private PlayerStateEvent PlayerStateEvent { get; set; }
    private async void Awake()
    {
        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerStateEvent.Invoke(new GenericStateBundle<PlayerStateBundle>()
        {
            StateBundle = new PlayerStateBundle()
            {
                PlayerAttackState = new State<AttackState, bool>() { Reset = new ResetSystem() { State = ResetState.COMPLETE_RESET } },
                PlayerMovementState = new State<MovementState, MovementDto>() { Reset = new ResetSystem() { State = ResetState.COMPLETE_RESET } },
                PlayerActionState = new State<ActionState, bool>() { Reset = new ResetSystem() { State = ResetState.COMPLETE_RESET } }
            }
        });
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