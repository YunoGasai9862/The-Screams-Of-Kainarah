using GlobalAccessAndGameHelper;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver
{
    public void CancelAction()
    {
        throw new System.NotImplementedException();
    }

    public void PerformAction()
    {
        _ = HandleJumping();
    }

    public Task HandleJumping()
    {
        if (CanPlayerJump()) //jumping
        {
            globalVariablesAccess.ISJUMPING = true;

            characterVelocityY = JumpSpeed * .5f;

            _animationHandler.JumpingFalling(globalVariablesAccess.ISJUMPING); //jumping animation
        }

        if (CanPlayerFall() || MaxJumpTimeChecker()) //peak reached
        {
            globalVariablesAccess.ISJUMPING = false;

            characterVelocityY = -JumpSpeed * .8f;

            isJumpPressed = false; //fixed the issue of eternally looping at jumep on JUMP HOLD

        }

        if (!globalVariablesAccess.ISJUMPING && !LedgeGroundChecker(groundLayer, ledgeLayer)) //falling
        {
            characterVelocityY = -JumpSpeed * .8f; //extra check
            _animationHandler.JumpingFalling(globalVariablesAccess.ISJUMPING); //falling naimation

        }

        if (_rb.velocity.y > 0f) //how high the player can jump
        {
            _timeCounter += Time.deltaTime;
        }

        if (!globalVariablesAccess.ISJUMPING && LedgeGroundChecker(groundLayer, ledgeLayer) && !isJumpPressed) //on the ground
        {
            characterVelocityY = 0f;
            _timeCounter = 0;
        }

        return Task.CompletedTask;

    }

    private Task<bool> CanPlayerJump()
    {
        bool _isJumping = globalVariablesAccess.ISJUMPING;
        bool _isOntheLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool _isJumpPressed = isJumpPressed;

        return Task.FromResult(globalVariablesAccess.boolConditionAndTester(!_isJumping, _isOntheLedgeOrGround, _isJumpPressed));
    }

    private Task<bool> CanPlayerFall()
    {
        bool _isJumping = globalVariablesAccess.ISJUMPING;
        bool _isOntheLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool _isJumpPressed = isJumpPressed;

        return Task.FromResult(globalVariablesAccess.boolConditionAndTester(_isJumping, !_isOntheLedgeOrGround, !_isJumpPressed));
    }

}
