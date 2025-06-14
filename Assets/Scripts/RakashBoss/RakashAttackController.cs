using System.Collections;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<AttackAnimationPackage, ActionExecuted>
{
    private AnimationUtility AnimationUtility { get; set; }

    private RakashAttacks CurrentAttackingState { get; set; }
    private void Start()
    {
        AnimationUtility = new AnimationUtility();  
    }

    ActionExecuted IReceiver<AttackAnimationPackage, ActionExecuted>.PerformAction(AttackAnimationPackage value)
    {
        //test why the animation is being executed so many times
        //but this whole architecture is an improvement so yay!
        StartCoroutine(Attack(value));

        return new ActionExecuted();
    }

    ActionExecuted IReceiver<AttackAnimationPackage, ActionExecuted>.CancelAction()
    {
        return new ActionExecuted { };
    }

    private IEnumerator Attack(AttackAnimationPackage value)
    {
        //test this and block if its already attacking
        CurrentAttackingState = RakashAttacks.ATTACK;

        yield return new WaitForSeconds(value.AttackDelay);

        AnimationUtility.ExecuteAnimation(value.Animation, value.Animator);

        CurrentAttackingState = RakashAttacks.NOT_ATTACKING;
    }
}