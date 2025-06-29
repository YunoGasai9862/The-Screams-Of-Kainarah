using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashBattleController : MonoBehaviour, IObserver<Health>, IReceiver<BattleActionDelegatePackage, Task<ActionExecuted>>
{
    [SerializeField]
    HealthDelegator healthDelegator;
    [SerializeField]
    GameObject rakashDeadBodyPrefab;

    private AnimationUtility AnimationUtility { get; set; }

    private List<RakashAttack> BlockingAttacks { get; set; }


    private const float HEALTH_DEPLETED_MARK = 0;

    private const float DESTROY_DELAY = 1.0f;

    private Health RakashHealth { get; set; }

    private void Start()
    {
        AnimationUtility = new AnimationUtility();

        BlockingAttacks = new List<RakashAttack>()
        {
           RakashAttack.ATTACK,

           RakashAttack.ATTACK_02
        };

        StartCoroutine(healthDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = typeof(RakashManager).ToString()

        }, CancellationToken.None));
    }

    private IEnumerator Attack(AttackAnimationPackage value)
    {
        yield return new WaitForSeconds(value.AttackDelay);

        AnimationUtility.ExecuteAnimations(value.Animations, value.Animator);
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

    private async Task<ActionExecuted> DelegateAction(BattleActionDelegatePackage battleActionDelegatePackage)
    {
        switch (battleActionDelegatePackage.AttackActionDelegate)
        {
            case BattleActionDelegate.ATTACK:
                return await AttactAction(battleActionDelegatePackage.AttackAnimationPackage);

            case BattleActionDelegate.TAKE_HIT:
                return await TakeHitAction(battleActionDelegatePackage.AttackAnimationPackage);

            case BattleActionDelegate.DESTROY_ON_DEFEAT:
                break;

            case BattleActionDelegate.ENTITY_DEFEATED:
                break;
        }

        return new ActionExecuted { };
    }

    private async Task<ActionExecuted> AttactAction(AttackAnimationPackage attackAnimationPackage)
    {
        if (await IsAnAttack(BlockingAttacks, attackAnimationPackage.AnimatorStateInfo))
        {
            return new ActionExecuted();
        }

        StartCoroutine(Attack(attackAnimationPackage));

        return new ActionExecuted();
    }

    private async Task<ActionExecuted> TakeHitAction(AttackAnimationPackage attackAnimationPackage)
    {
        await AnimationUtility.ExecuteAnimations(attackAnimationPackage.Animations, attackAnimationPackage.Animator);

        if (RakashHealth.CurrentHealth == HEALTH_DEPLETED_MARK)
        {
            ActionExecuted<GameObject> actionExecuted = await EntityDefeatedAction(rakashDeadBodyPrefab);

            await DestroyOnDefeatAction(new List<GameObject>() { actionExecuted.Result, gameObject }, DESTROY_DELAY);

            return new ActionExecuted();
        }

        RakashHealth.CurrentHealth -= attackAnimationPackage.AttackPoints;

        return new ActionExecuted();
    }

    private Task<ActionExecuted> DestroyOnDefeatAction(List<GameObject> objectsToDestroy, float delay) {

        Helper.DestroyMultipleGameObjects(objectsToDestroy, delay);

        return Task.FromResult(new ActionExecuted()); 
    }

    private async Task<ActionExecuted<GameObject>> EntityDefeatedAction(GameObject deadBodyPrefab) {

        GameObject instantiatedObject = await Helper.InstantiatePrefabAt(transform.position, deadBodyPrefab);

       return new ActionExecuted<GameObject>(instantiatedObject);

    }

    public void OnNotify(Health data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        RakashHealth = data;
    }
}