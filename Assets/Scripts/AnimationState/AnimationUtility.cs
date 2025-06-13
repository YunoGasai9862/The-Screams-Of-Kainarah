using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class AnimationUtility
{
    public void ExecuteAnimation(Animation animation, Animator animator)
    {
        string description = ResolveAnimationName(animation);

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

            default:
                break;
        }
    }

    private string ResolveAnimationName(Animation animation)
    {
        //gets the actual fieldInfo for that particular Enum value 
        FieldInfo fieldValue = animation.GetType().GetField(animation.ToString());

        DescriptionAttribute attribute = fieldValue.GetCustomAttribute<DescriptionAttribute>();

        return attribute == null? null : attribute.Description.ToString();
    }
}