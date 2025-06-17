using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RakashAttackController : MonoBehaviour, IReceiver<AttackAnimationPackage, Task<ActionExecuted>>
{
    private AnimationUtility AnimationUtility { get; set; }

    private List<RakashAttack> BlockingAttacks { get; set; }

    private void Start()
    {
        AnimationUtility = new AnimationUtility();

        BlockingAttacks = new List<RakashAttack>()
        {
           RakashAttack.ATTACK,

           RakashAttack.ATTACK_02
        };
    }

    private IEnumerator Attack(AttackAnimationPackage value)
    {
        yield return new WaitForSeconds(value.AttackDelay);

        AnimationUtility.ExecuteAnimation(value.Animation, value.Animator);
    }

    private async Task<bool> IsAnAttack(List<RakashAttack> rakashAttacks, AnimatorStateInfo info)
    {
        foreach(RakashAttack attack in rakashAttacks)
        {
            if (info.IsTag(await AnimationUtility.ResolveAnimationName(attack)))
            {
                Debug.Log("YESS");

                return true;
            }
        }

        return false;
    }

    async Task<ActionExecuted> IReceiver<AttackAnimationPackage, Task<ActionExecuted>>.PerformAction(AttackAnimationPackage value)
    {
        if (await IsAnAttack(BlockingAttacks, value.AnimatorStateInfo))
        {
            return new ActionExecuted();
        }

        StartCoroutine(Attack(value));

        return new ActionExecuted();
    }

    async Task<ActionExecuted> IReceiver<AttackAnimationPackage, Task<ActionExecuted>>.CancelAction()
    {
        return await Task.FromResult(new ActionExecuted { });
    }
}