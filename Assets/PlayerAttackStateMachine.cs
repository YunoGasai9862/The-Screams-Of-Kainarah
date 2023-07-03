using UnityEngine;

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

    public bool istheCurrentAnimationPlaying()
    {
        return this._animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f; //if its playing or not
    }



}
