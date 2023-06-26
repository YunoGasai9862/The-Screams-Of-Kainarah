using UnityEngine;

public class AnimationStateMachine
{
    private Animator animator; //each object will have its own AnimationStateMachine

    public AnimationStateMachine(Animator animator)
    {
        this.animator = animator;
    }

    public void AnimationPlayMachineInt(string constName, int state)
    {
        this.animator.SetInteger(constName, state);

    }
    public void AnimationPlayMachineBool(string constName, bool state)
    {
        this.animator.SetBool(constName, state);
    }

}