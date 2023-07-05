using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private AnimationStateMachine _stateMachine;
    private Animator _anim;
    private float maxSlideTime = 0.4f;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        if (_anim != null)
            _stateMachine = new AnimationStateMachine(_anim); // initializing the object
    }

    private void Update()
    {
        if (this._anim.GetCurrentAnimatorStateInfo(0).IsName(AnimationConstants.SLIDING) &&
            returnCurrentAnimation() > maxSlideTime)
        {
            PlayAnimation(AnimationConstants.SLIDING, false);  //for fixing the Sliding Issue
        }
    }

    public bool VectorChecker(float compositionX)
    {
        return compositionX != 0f;
    }

    private void PlayAnimation(string name, int state)
    {
        _stateMachine.AnimationPlayMachineInt(name, state);
    }

    private void PlayAnimation(string name, bool state)
    {
        _stateMachine.AnimationPlayMachineBool(name, state);
    }

    public void RunningWalkingAnimation(float keystroke)
    {
        AnimationStateKeeper.StateKeeper state;

        if (VectorChecker(keystroke))
        {
            state = AnimationStateKeeper.StateKeeper.RUNNING;
            globalVariablesAccess.ISRUNNING = true;
            globalVariablesAccess.ISWALKING = false;
        }
        else
        {
            state = AnimationStateKeeper.StateKeeper.IDLE;
            globalVariablesAccess.ISRUNNING = false;
            globalVariablesAccess.ISWALKING = true;
        }


        PlayAnimation(AnimationConstants.MOVEMENT, (int)state);
    }

    public void JumpingFalling(bool keystroke)
    {
        AnimationStateKeeper.StateKeeper state = keystroke
            ? AnimationStateKeeper.StateKeeper.JUMP
            : AnimationStateKeeper.StateKeeper.FALL;

        PlayAnimation(AnimationConstants.MOVEMENT, (int)state);
    }

    public void Sliding(bool keystroke)
    {
        PlayAnimation(AnimationConstants.SLIDING, keystroke);
    }

    public float returnCurrentAnimation()
    {
        return this._anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }


}
