
using Assets.Scripts.Scene;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PlayerAnimationHandler
{
    public class AnimationStateMachine: Scene
    {
        public void SetAnimation<T>(Animator animator, string stateName, T value)
        {

            switch (value)
            {
                case bool val:
                    animator.SetBool(stateName, val);
                    break;
                case float val:
                    animator.SetFloat(stateName, val);
                    break;
                case int val:
                    animator.SetInteger(stateName, val);
                    break;
            }
        }

        public void ResetParameters(Animator animator)
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter.name, 0f);
                        break;

                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter.name, false);
                        break;

                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter.name, 0);
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        animator.ResetTrigger(animator.GetInteger(parameter.name));
                        break;

                    default:
                        throw new System.Exception($"Unknown type: {parameter.type}");
                }
            }
        }

        public void ResetParameters( Animator animator, List<Reset> resetParameters, ResetState state)
        {
            AnimatorControllerParameter[] animatorControllerParameters = animator.parameters.ToArray();

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
                        animator.SetFloat(reset.m_key, state.Equals(ResetState.REVERT) ? (float) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) : 
                            (float) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(reset.m_key, state.Equals(ResetState.REVERT) ? (bool) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) :
                            (bool) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(reset.m_key, state.Equals(ResetState.REVERT) ? (int) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_oldValue) :
                            (int) SceneUtils.Convert(reset.m_val.m_type, reset.m_val.m_newValue));
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        animator.ResetTrigger(animator.GetInteger(reset.m_key));
                        break;

                    default:
                        throw new System.Exception($"Unknown type: {reset.m_val.m_type}");
                }
            }
        }
    }
}
