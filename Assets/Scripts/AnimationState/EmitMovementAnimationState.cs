using Assets.Scripts.BaseScene;
using System.Threading;
using UnityEngine;

public class EmitMovementAnimationState : StateMachineScene
{
    private StateEvent StateEvent { get; set; }
    private async void Awake()
    {
        StateEvent = await (await BaseScene.GetSceneUtilsAsync()).GetCustomEvent<StateEvent>();
    }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateEvent?.Invoke(new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
        {
            StateBundle = new EmitAnimationStateBundle<bool>()
            {
                CurrentAnimation = new EmitAnimationStateBundle<bool>.CurrentAnimationInfo<bool>()
                {
                    CurrentValue = true,

                    CurrentAnimatorStateInfo = stateInfo,
                }
            }
        }); 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateEvent?.Invoke(new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
        {
            StateBundle = new EmitAnimationStateBundle<bool>()
            {
                CurrentAnimation = new EmitAnimationStateBundle<bool>.CurrentAnimationInfo<bool>()
                {
                    CurrentValue = false,

                    CurrentAnimatorStateInfo = stateInfo,
                }
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
