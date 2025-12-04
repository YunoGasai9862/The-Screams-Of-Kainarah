
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

        public void ResetParameters(List<Reset> resetParameters, ResetState state)
        {
            AnimatorControllerParameter[] animatorControllerParameters = _animator.parameters.ToArray();

            foreach (Reset reset in resetParameters)
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
                        _animator.SetFloat(reset.m_key, state.Equals(ResetState.REVERT) ? (float) Helper.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) : 
                            (float) Helper.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Bool:
                        _animator.SetBool(reset.m_key, state.Equals(ResetState.REVERT) ? (bool) Helper.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) :
                            (bool) Helper.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Int:
                        _animator.SetInteger(reset.m_key, state.Equals(ResetState.REVERT) ? (int) Helper.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) :
                            (int) Helper.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        _animator.ResetTrigger(_animator.GetInteger(reset.m_key));
                        break;

                    default:
                        throw new System.Exception($"Unknown type: {reset.m_val.m_type}");
                }
            }
        }
    }
}
