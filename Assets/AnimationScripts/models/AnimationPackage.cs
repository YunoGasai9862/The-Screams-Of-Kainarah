using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationPackage
{
    public List<Animation> Animations { get; set; }
    public Animator Animator { get; set; }
    public AnimatorStateInfo AnimatorStateInfo { get; set; }
}