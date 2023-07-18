
using GlobalAccessAndGameHelper;
using UnityEngine;
namespace PlayerAnimationHandler
{
    public class AnimationConstants
    {

        public const string MOVEMENT = "State";
        public const string SLIDING = "Sliding";
        public const string LEDGEGRAB = "LedgeGrab";
        public const string THROWDAGGER = "ThrowDagger";


    }
    public class AnimationHandler : MonoBehaviour
    {
        private AnimationStateMachine _stateMachine;
        private Animator _anim;
        private float maxSlideTime = 0.4f;

        private void Awake()
        {
            _anim = GetComponent<Animator>();

            if (_anim != null)
                _stateMachine = new AnimationStateMachine(_anim); // initializing the object
        }

        private void Update()
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName(AnimationConstants.SLIDING) &&
                returnCurrentAnimation() > maxSlideTime)
            {
                PlayAnimation(AnimationConstants.SLIDING, false);  //for fixing the Sliding Issue
            }
        }

        public bool VectorChecker(float compositionX)
        {
            return compositionX != 0f;
        }

        private void PlayAnimation(string name, int state)
        {
            _stateMachine.AnimationPlayMachineInt(name, state);
        }

        private void PlayAnimation(string name, bool state)
        {
            _stateMachine.AnimationPlayMachineBool(name, state);
        }

        public void RunningWalkingAnimation(float keystroke)
        {
            AnimationStateKeeper.StateKeeper state;

            if (VectorChecker(keystroke))
            {
                state = AnimationStateKeeper.StateKeeper.RUNNING;
                globalVariablesAccess.ISRUNNING = true;
                globalVariablesAccess.ISWALKING = false;
            }
            else
            {
                state = AnimationStateKeeper.StateKeeper.IDLE;
                globalVariablesAccess.ISRUNNING = false;
                globalVariablesAccess.ISWALKING = true;
            }


            PlayAnimation(AnimationConstants.MOVEMENT, (int)state);
        }

        public void JumpingFalling(bool keystroke)
        {
            AnimationStateKeeper.StateKeeper state = keystroke
                ? AnimationStateKeeper.StateKeeper.JUMP
                : AnimationStateKeeper.StateKeeper.FALL;
            PlayAnimation(AnimationConstants.MOVEMENT, (int)state);
        }

        public void Sliding(bool keystroke)
        {
            PlayAnimation(AnimationConstants.SLIDING, keystroke);
        }

        public float returnCurrentAnimation()
        {
            return _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public bool isNameOfTheCurrentAnimation(string name)
        {
            return _anim.GetCurrentAnimatorStateInfo(0).IsName(name);
        }

        public Animator getAnimator()
        {
            return _anim;
        }


    }

    public class AnimationStateKeeper
    {
        public static int currentPlayerState = 0;

        public enum StateKeeper
        {
            IDLE = 0, RUNNING = 1, JUMP = 2, FALL = 3, SLIDING = 4
        }


    }

    public class AnimationStateMachine
    {
        private Animator animator; //each object will have its own AnimationStateMachine

        public AnimationStateMachine(Animator animator)
        {
            this.animator = animator;
        }

        public void AnimationPlayMachineInt(string constName, int state)
        {
            animator.SetInteger(constName, state);

        }
        public void AnimationPlayMachineBool(string constName, bool state)
        {
            animator.SetBool(constName, state);
        }

    }


}
