
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
                switch (parameter.type)
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
                AnimatorControllerParameter animatorControllerParameter = animatorControllerParameters.FirstOrDefault(acp => acp.name == reset.m_key);

                if (animatorControllerParameter == null)
                {
                    Debug.Log($"m_key: {reset.m_key} is absent from the AnimatorControllerParameter list!");
                    continue;
                }

                switch (reset.m_val.m_type)
                {
                    case AnimatorControllerParameterType.Float:
                        _animator.SetFloat(reset.m_key, state.Equals(ResetSystem.ResetState.REVERT) ? (float) Convert(reset.m_val.m_oldValue) : (float) Convert(reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Bool:
                        _animator.SetBool(reset.m_key, state.Equals(ResetSystem.ResetState.REVERT) ? (bool) Convert(reset.m_val.m_oldValue) : (bool) Convert(reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Int:
                        _animator.SetInteger(reset.m_key, state.Equals(ResetSystem.ResetState.REVERT) ? (int) Convert(reset.m_val.m_oldValue) : (int)Convert(reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        _animator.ResetTrigger(_animator.GetInteger(reset.m_key));
                        break;

                    default:
                        throw new System.Exception($"Unknown type: {reset.m_val.m_type}");
                }
            }
        }

        //TODO!!!
        private dynamic Convert(ResetSystem.Reset.Field field)
        {
            return null;
        }

    }

}
