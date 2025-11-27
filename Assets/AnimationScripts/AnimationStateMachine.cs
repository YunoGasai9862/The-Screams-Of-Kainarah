
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PlayerAnimationHandler
{
    public class AnimationStateMachine
    {
        private Animator _animator;

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

        public void ResetParameters()
        {
            foreach (AnimatorControllerParameter parameter in _animator.parameters)
            {
                switch(parameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        _animator.SetFloat(parameter.name, 0f);
                        break;

                    case AnimatorControllerParameterType.Bool:
                        _animator.SetBool(parameter.name, false);
                        break;

                    case AnimatorControllerParameterType.Int:
                        _animator.SetInteger(parameter.name, 0);
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        _animator.ResetTrigger(_animator.GetInteger(parameter.name)); 
                        break;

                    default:
                        throw new System.Exception($"Unknown type: {parameter.type}");
                }
            }
        }

        public void ResetParameters(List<ResetSystem.Reset> resetParameters, ResetSystem.ResetState state)
        {
            AnimatorControllerParameter[] animatorControllerParameters = _animator.parameters.ToArray();

            foreach (ResetSystem.Reset reset in resetParameters)
            {
                AnimatorControllerParameter animatorControllerParameter = animatorControllerParameters.FirstOrDefault(acp => acp.name == reset.Key);

                if (animatorControllerParameter == null)
                {
                    Debug.Log($"Key: {reset.Key} is absent from the AnimatorControllerParameter list!");
                    continue;
                }

                switch (reset.Val.Type)
                {
                    case AnimatorControllerParameterType.Float:
                        _animator.SetFloat(reset.Key, state.Equals(ResetSystem.ResetState.REVERT) ? reset.Val.OldValue : reset.Val.NewValue);
                        break;

                    case AnimatorControllerParameterType.Bool:
                        _animator.SetBool(reset.Key, state.Equals(ResetSystem.ResetState.REVERT) ? reset.Val.OldValue : reset.Val.NewValue);
                        break;

                    case AnimatorControllerParameterType.Int:
                        _animator.SetInteger(reset.Key, state.Equals(ResetSystem.ResetState.REVERT) ? reset.Val.OldValue : reset.Val.NewValue);
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        _animator.ResetTrigger(_animator.GetInteger(reset.Key));
                        break;

                    default:
                        throw new System.Exception($"Unknown type: {reset.Val.Type}");
                }
            }
        }
    }
}
