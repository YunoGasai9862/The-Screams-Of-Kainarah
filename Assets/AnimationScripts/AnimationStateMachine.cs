
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;
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
        public void SetAnimation<T>(string stateName, T value)
        {

            switch (value)
            {
                case bool val:
                    _animator.SetBool(stateName, val);
                    break;
                case float val:
                    _animator.SetFloat(stateName, val);
                    break;
                case int val:
                    _animator.SetInteger(stateName, val);
                    break;
            }
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
                        _animator.SetFloat(reset.m_key, state.Equals(ResetState.REVERT) ? (float) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) : 
                            (float) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Bool:
                        _animator.SetBool(reset.m_key, state.Equals(ResetState.REVERT) ? (bool) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) :
                            (bool) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Int:
                        _animator.SetInteger(reset.m_key, state.Equals(ResetState.REVERT) ? (int) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) :
                            (int) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
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
