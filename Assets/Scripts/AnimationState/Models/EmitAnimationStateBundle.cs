using UnityEngine;

public class EmitAnimationStateBundle : IStateBundle
{
    public bool IsRunning { get; set; }

    public AnimatorStateInfo AnimatorStateInfo { get; set; }
}