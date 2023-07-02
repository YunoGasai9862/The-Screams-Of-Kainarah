using UnityEngine;

public class PlayerAttackStateMachine
{
    private Animator _animator;
    private string currentStateName;

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

    public void setAttackState(string parameterName, int currentStateInt, string currentStateName)
    {
        this.currentStateName = currentStateName;
        this._animator.SetInteger(parameterName, currentStateInt);
    }

    public void timeDifferenceRequiredBetweenTwoStates(string parameterName, float timePassed)
    {
        this._animator.SetFloat(parameterName, timePassed);
    }

    public void canAttack(string parameterName, bool canAttack)
    {
        this._animator.SetBool(parameterName, canAttack);
    }


}
