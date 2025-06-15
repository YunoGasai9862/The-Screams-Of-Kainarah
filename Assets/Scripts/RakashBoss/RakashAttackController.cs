using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<AttackAnimationPackage, ActionExecuted>
{
    private AnimationUtility AnimationUtility { get; set; }

    private List<RakashAttacks> BlockingAttacks { get; set; }

    private void Start()
    {
        AnimationUtility = new AnimationUtility();

        BlockingAttacks = new List<RakashAttacks>()
        {
           RakashAttacks.ATTACK,

           RakashAttacks.ATTACK_02
        };
    }

    ActionExecuted IReceiver<AttackAnimationPackage, ActionExecuted>.PerformAction(AttackAnimationPackage value)
    {
        
        if (IsAnimationStateInfoFromRaskashAttacks(BlockingAttacks, value.AnimatorStateInfo))
        {
            return new ActionExecuted();
        }

        StartCoroutine(Attack(value));

        return new ActionExecuted();
    }

    ActionExecuted IReceiver<AttackAnimationPackage, ActionExecuted>.CancelAction()
    {
        return new ActionExecuted { };
    }

    private IEnumerator Attack(AttackAnimationPackage value)
    {
        yield return new WaitForSeconds(value.AttackDelay);

        AnimationUtility.ExecuteAnimation(value.Animation, value.Animator);
    }

    private bool IsAnimationStateInfoFromRaskashAttacks(List<RakashAttacks> rakashAttacks, AnimatorStateInfo info)
    {
        foreach(RakashAttacks attack in rakashAttacks)
        {
            if (info.IsTag(AnimationUtility.ResolveAnimationName(attack)))
            {
                Debug.Log("YESS");

                return true;
            }
        }

        return false;
    }
}