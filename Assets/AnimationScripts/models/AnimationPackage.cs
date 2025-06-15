using UnityEngine;

public abstract class AnimationPackage
{
    public Animation Animation { get; set; }
    public Animator Animator { get; set; }
    public AnimatorStateInfo AnimatorStateInfo { get; set; }
}