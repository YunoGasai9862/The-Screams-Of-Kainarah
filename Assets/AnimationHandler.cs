using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private AnimationStateMachine _stateMachine;
    private Animator _anim;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _stateMachine = new AnimationStateMachine(_anim); //initializing the object
    }
    private bool VectorChecker(float CompositionX)
    {
        return (CompositionX > 0f || CompositionX < 0f);
    }

    private bool keystrokeChecker(bool keystroke)
    {
        return keystroke;
    }

    private void PlayAnimation(string Name, int State)
    {
        _stateMachine.AnimationPlayMachineInt(Name, State);

    }
    public void RunningWalkingAnimation(float keystroke)
    {

        _ = VectorChecker(keystroke) ?
            AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.RUNNING :
            AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.IDLE;

        PlayAnimation(AnimationConstants.MOVEMENT, AnimationStateKeeper.currentPlayerState);

    }

    public void JumpingFalling(bool keystroke)
    {
        _ = keystrokeChecker(keystroke) ? AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.JUMP : AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.FALL;

        PlayAnimation(AnimationConstants.MOVEMENT, AnimationStateKeeper.currentPlayerState);

    }

    public void Sliding(bool keystroke)
    {
        _ = keystrokeChecker(keystroke) ? AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.SLIDING : AnimationStateKeeper.currentPlayerState = (int)AnimationStateKeeper.StateKeeper.RUNNING;

        PlayAnimation(AnimationConstants.SLIDING, AnimationStateKeeper.currentPlayerState);
    }



}
