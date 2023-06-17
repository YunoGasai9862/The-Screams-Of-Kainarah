using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationStateMachine
{
    private Animator animator; //each object will have its own AnimationStateMachine

    public AnimationStateMachine(Animator animator)
    {
        this.animator = animator;
    }

    public void AnimationPlayMachine(string constName, int state)
    {
        this.animator.SetInteger(constName, state);

    }

}