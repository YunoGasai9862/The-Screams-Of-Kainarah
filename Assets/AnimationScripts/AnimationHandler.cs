
using UnityEngine;
namespace PlayerAnimationHandler
{
    public class AnimationConstants
    {
        public const string MOVEMENT = "State";
        public const string SLIDING = "Sliding";
        public const string LEDGE_GRAB = "LedgeGrab";
        public const string THROW_DAGGER = "ThrowDagger";
        public const string JUMP_TIME = "JumpTime";
    }
    public class AnimationStateKeeper
    {
        private static int _currentPlayerState= 0;
        public static int CurrentPlayerState { get => _currentPlayerState; set=> _currentPlayerState = value; }
        public enum StateKeeper
        {
            IDLE = 0, RUNNING = 1, JUMP = 2, FALL = 3, SLIDING = 4
        }

    }
    public class AnimationStateMachine
    {
        private Animator _animator; //each object will have its own AnimationStateMachine

        public AnimationStateMachine(Animator animator)
        {
            this._animator = animator;
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
