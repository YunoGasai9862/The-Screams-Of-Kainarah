using System;
using UnityEngine;
namespace CoreCode
{
    public class PlayerAttackStateMachine
    {
        private Animator _animator;

        public PlayerAttackStateMachine(Animator animator)
        {
            this._animator = animator;
        }

        public Animator GetAnimator()
        {
            return _animator;
        }

        public void SetAnimator(Animator animator)
        {
            this._animator = animator;
        }

        public void SetAttackState(string parameterName, int currentStateInt)
        {
            _animator.SetInteger(parameterName, currentStateInt);
        }

        public void SetAttackState(string parameterName, bool currentStateBool)
        {
            _animator.SetBool(parameterName, currentStateBool);
        }

        public void TimeDifferenceRequiredBetweenTwoStates(string parameterName, float timePassed)
        {
            _animator.SetFloat(parameterName, timePassed);
        }

        public void CanAttack(string parameterName, bool canAttack)
        {
            _animator.SetBool(parameterName, canAttack);
        }

        public bool IstheAttackCancelConditionTrue(string stateName, string[] expectedStateName)
        {
            foreach (string expectedStateNames in expectedStateName)
            {

                if (stateName == expectedStateNames)
                {
                    return _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f;
                }

            }

            return false;

        }

        public bool GetCurrentState(string state)
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName(state);
        }

        public string GetStateNameThroughEnum(int state)
        {
            return Enum.GetName(typeof(PlayerAttackEnum.PlayerAttackSlash), state);
        }
        public void ForceDisableAttacking(int state)
        {
            string stateName = GetStateNameThroughEnum(state);
            SetAttackState(stateName, false);
        }

        public bool IsInEitherOfTheAttackingStates<T>()
        {
            bool result = false;

            for (int i = 0; i < Enum.GetNames(typeof(T)).Length - 1; i++)
            {
                string stateName = GetStateNameThroughEnum(i + 1);

                result = result || _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);

            }

            return result;

        }

    }

}
