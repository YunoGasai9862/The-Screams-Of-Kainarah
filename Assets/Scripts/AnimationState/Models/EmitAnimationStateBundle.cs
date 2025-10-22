using UnityEngine;

public class EmitAnimationStateBundle<T> : IStateBundle
{
    public class CurrentAnimationInfo<T>
    {
        public T CurrentValue { get; set; }

        public AnimatorStateInfo CurrentAnimatorStateInfo { get; set; }

        public CurrentAnimationInfo<T> Copy(T value, AnimatorStateInfo animatorStateInfo)
        {
            return new CurrentAnimationInfo<T> { CurrentValue = value, CurrentAnimatorStateInfo = animatorStateInfo };
        }
    }

    public class PreviousAnimationInfo
    {
        public int PreviousAnimationHash { get; set; }
    }

    public CurrentAnimationInfo<T> CurrentAnimation { get; set; }

    public PreviousAnimationInfo PreviousAnimation { get; set; }
}
