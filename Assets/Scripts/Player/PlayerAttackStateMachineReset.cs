using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerAttackStateMachineReset : StateMachineBehaviour, IObserver<EntityPoolManager>
{
    //fetch from entity pool manager
    private AttackResetConfig AttackResetConfig { get; set; }

    private IReceiverEnhancedAsync<PlayerAnimationResetController, State<AttackState>> PlayerAnimationStateControllerAS { get; set; }

    private CommandAsyncEnhanced<PlayerAnimationResetController, State<AttackState>> PlayerAnimationResetControllerCommandAS { get; set; }


    private async void Awake()
    {
        PlayerAnimationStateControllerAS = await Helper.FindReceiver<PlayerAnimationResetController, IReceiverEnhancedAsync<PlayerAnimationResetController, State<AttackState>>>();

        PlayerAnimationResetControllerCommandAS = new CommandAsyncEnhanced<PlayerAnimationResetController, State<AttackState>>(PlayerAnimationStateControllerAS);
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
    override async public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        await PlayerAnimationResetControllerCommandAS.Execute(new State<AttackState>()
        {
            Reset = new ResetSystem()
            {
                resetParameters = new List<Reset> ()
                {
                   //use the value exactly from the ResetConfig - the type
                },
                state = ResetState.PARTIAL_RESET
            }
        });
    }

    public void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        throw new System.NotImplementedException();
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