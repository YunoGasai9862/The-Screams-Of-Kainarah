using UnityEngine;

public class EmitAnimationStateBundle<T> : IStateBundle
{
    public T Value { get; set; }

    public AnimatorStateInfo AnimatorStateInfo { get; set; }
}