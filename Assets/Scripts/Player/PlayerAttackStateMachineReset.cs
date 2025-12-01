using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackStateMachineReset : StateMachineBehaviour
{
    [SerializeField]
    ResetConfig resetConfig;

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
            //FIX LATER - CANT BE INITIALIZED!!
            Reset = new ResetSystem()
            {
                resetParameters = new List<Reset> ()
                {
                   //use the value exactly from the ResetConfig - the type
                },
                state = ResetSystem.ResetState.PARTIAL_RESET
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