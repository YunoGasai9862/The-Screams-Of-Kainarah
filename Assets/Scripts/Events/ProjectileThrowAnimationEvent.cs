using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileThrowAnimationEvent: UnityEvent<bool>
{
    public static ProjectileThrowAnimationEvent Instance = new ProjectileThrowAnimationEvent();
    public ProjectileThrowAnimationEvent() { }

    public Task<bool> IsAnimationTimeHasPassed(Animator anim, string animationName, float time)
    {
        return Task.FromResult(anim.GetCurrentAnimatorStateInfo(0).IsName(animationName)
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > time);
    }

    public async Task InvokeEvent(Animator anim, string animationName, float time)
    {
        Instance.Invoke(await IsAnimationTimeHasPassed(anim, animationName, time));
    }
}