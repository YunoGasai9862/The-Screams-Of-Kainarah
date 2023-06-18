using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private AnimationStateMachine _stateMachine;
    private Animator _anim;


    private void Awake()
    {
        _anim= GetComponent<Animator>();
        _stateMachine = new AnimationStateMachine(_anim); //initializing the object
    }
    private bool VectorChecker(float CompositionX)
    {
        return (CompositionX > 0f || CompositionX < 0f);
    }

    private void PlayAnimation(string Name, int State)
    {
        _stateMachine.AnimationPlayMachine(Name, State);

    }
    public void RunningWalkingAnimation(float keystroke)
    {

        _ = VectorChecker(keystroke) ?
            AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.RUNNING :
            AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.IDLE;

            PlayAnimation(AnimationConstants.MOVEMENT, AnimationStateKeeper.currentPlayerState);
      
    }



}
