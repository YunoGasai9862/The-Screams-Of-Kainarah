using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RakashBattleController : MonoBehaviour, IReceiver<BattleActionDelegatePackage, Task<ActionExecuted>>
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

    async Task<ActionExecuted> IReceiver<BattleActionDelegatePackage, Task<ActionExecuted>>.PerformAction(BattleActionDelegatePackage value)
    {
        if (await IsAnAttack(BlockingAttacks, value.AttackAnimationPackage.AnimatorStateInfo))
        {
            return new ActionExecuted();
        }

        StartCoroutine(Attack(value.AttackAnimationPackage));

        return new ActionExecuted();
    }

    async Task<ActionExecuted> IReceiver<BattleActionDelegatePackage, Task<ActionExecuted>>.CancelAction()
    {
        return await Task.FromResult(new ActionExecuted { });
    }

    private Task DelegateAction(BattleActionDelegate battleActionDelegate)
    {
        switch (battleActionDelegate)
        {
            case BattleActionDelegate.ATTACK:
                break;

            case BattleActionDelegate.TAKE_HIT:
                break;

            case BattleActionDelegate.DESTROY_ON_DEFEAT:
                break;

            case BattleActionDelegate.ENTITY_DEFEATED:
                break;
        }

        return Task.CompletedTask;
    }

    private Task AttactAction() { return Task.CompletedTask; }

    private Task TakeHitAction() { return Task.CompletedTask; }

    private Task DestroyOnDefeatAction() { return Task.CompletedTask; }

    private Task EntityDefeatedAction() { return Task.CompletedTask; }
}