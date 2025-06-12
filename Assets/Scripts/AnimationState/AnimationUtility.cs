using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class AnimationUtility
{
    public void ExecuteAnimation(Animation animation, Animator animator)
    {
        switch (animation)
        {
            case Animation.START_WALK:
                animator.SetBool("walk", true);
                break;

            case Animation.STOP_WALK:
                animator.SetBool("walk", false);
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