using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationUtility
{
    public async void ExecuteAnimation(Animation animation, Animator animator)
    {
        string description = await ResolveAnimationName(animation);

        Debug.Log($"Name Found: {description}");

        switch (animation)
        {
            case Animation.START_WALK:
                animator.SetBool(description, true);
                break;

            case Animation.STOP_WALK:
                animator.SetBool(description, false);
                break;

            case Animation.START_ATTACK:
                animator.SetTrigger(description);
                break;

            case Animation.STOP_ATTACK:
                break;

            case Animation.TAKE_HIT:
                break;

            default:
                break;
        }
    }

    public Task<string> ResolveAnimationName<T>(T animation)
    {
        FieldInfo fieldValue = animation.GetType().GetField(animation.ToString());

        DescriptionAttribute attribute = fieldValue.GetCustomAttribute<DescriptionAttribute>();

        return attribute == null? null : Task.FromResult(attribute.Description.ToString());
    }
}