
using UnityEngine;
namespace PlayerAnimationHandler
{
    public class AnimationStateMachine
    {
        private Animator _animator; //each object will have its own AnimationStateMachine

        public AnimationStateMachine(Animator animator)
        {
            if (animator == null)
            {
                throw new System.Exception("Animator is null - can't initalize the AnimationStateMachine!");
            }

            _animator = animator;
        }
        public void AnimationPlayForInt(string constName, int state)
        {
            _animator.SetInteger(constName, state);
        }
        public void AnimationPlayForBool(string constName, bool state)
        {
            _animator.SetBool(constName, state);
        }
        public void AnimationPlayForFloat(string constName, float state)
        {
            _animator.SetFloat(constName, state);
        }
    }
}
