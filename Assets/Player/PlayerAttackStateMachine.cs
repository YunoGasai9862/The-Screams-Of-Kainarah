using System;
using UnityEngine;
namespace CoreCode
{
    public class PlayerAttackStateMachine
    {
        private Animator _animator;

        public PlayerAttackStateMachine(Animator _animator)
        {
            this._animator = _animator;
        }

        public Animator getAnimator()
        {
            return _animator;
        }

        public void setAnimator(Animator _animator)
        {
            this._animator = _animator;
        }

        public void setAttackState(string parameterName, int currentStateInt)
        {
            _animator.SetInteger(parameterName, currentStateInt);
        }

        public void setAttackState(string parameterName, bool currentStateInt)
        {
            _animator.SetBool(parameterName, currentStateInt);
        }


        public void timeDifferenceRequiredBetweenTwoStates(string parameterName, float timePassed)
        {
            Debug.Log(_animator);
            _animator.SetFloat(parameterName, timePassed);
        }

        public void canAttack(string parameterName, bool canAttack)
        {
            _animator.SetBool(parameterName, canAttack);
        }

        public bool istheAttackCancelConditionTrue(string stateName, string[] expectedStateName)
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

        public bool getCurrentState(string state)
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName(state);
        }

        public string getStateNameThroughEnum(int state)
        {
            return Enum.GetName(typeof(PlayerAttackEnum.PlayerAttackSlash), state);
        }
        public void ForceDisableAttacking(int state)
        {
            string stateName = getStateNameThroughEnum(state);
            setAttackState(stateName, false);

        }

        public bool isInEitherOfTheAttackingStates<T>()
        {
            bool result = false;

            for (int i = 0; i < Enum.GetNames(typeof(T)).Length - 1; i++)
            {
                string stateName = getStateNameThroughEnum(i + 1);

                result = result || _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);

            }

            return result;

        }

    }

}
